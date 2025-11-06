using BettingApi.Dto;
using BettingApi.Models;
using BettingApi.Repositories;

namespace BettingApi.Services
{
    // --------------------------
    // RNG — interface + impl
    // --------------------------
    public interface ICrashRng
    {
        double Generate();
    }

    public class CrashRng : ICrashRng
    {
        private readonly Random _rng = new();

        public double Generate()
        {
            double bias = 1.0;
            double r = _rng.NextDouble();
            double val = -Math.Log(1 - r) / bias;
            double rounded = Math.Round(val * 10) / 10;
            return Math.Max(1.03, rounded);
        }
    }


    // --------------------------
    // Crash Game Service
    // --------------------------
    public interface ICrashGameService
    {
        Task<CrashGameResultDto> CrashGamePlay(CrashGameRequestDto dto, int userAccountId);
        Task<CrashGameResultDto> CashOut(int userAccountId, double multiplierStoppedAt);
    }


    public class CrashGameService : ICrashGameService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICrashRng _rng;
        private readonly ICrashGameStateService _stateService;

        public CrashGameService(
            ITransactionRepository transactionRepository, 
            IUserRepository userRepository, 
            ICrashRng rng,
            ICrashGameStateService stateService)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _rng = rng;
            _stateService = stateService;
        }


        public async Task<CrashGameResultDto> CrashGamePlay(CrashGameRequestDto dto, int userAccountId)
        {
            var user = await _userRepository.GetByIdAsync(userAccountId);

            if (user == null || !user.ActiveStatus)
            {
                throw new Exception("User not active");
            }

            if (dto.BetAmount <= 0 || dto.BetAmount > user.Balance)
            {
                throw new Exception("Insufficient balance");
            }

            // Check if user already has an active game
            if (_stateService.TryGetGame(userAccountId, out _))
            {
                throw new Exception("You already have an active game running");
            }

            // Deduct bet from balance
            await _userRepository.UpdateBalanceByIdAsync(userAccountId, -dto.BetAmount);

            // Create loss transaction (bet amount)
            var betTransaction = new Transaction
            {
                UserAccountId = userAccountId,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Amount = -dto.BetAmount,
                GameName = "Crash"
            };

            await _transactionRepository.AddAsync(betTransaction);
            await _transactionRepository.SaveChangesAsync();

            // Generate crash point
            double crashPoint = _rng.Generate();

            // Store game state in singleton service
            if (!_stateService.CreateGame(userAccountId, dto.BetAmount, crashPoint))
            {
                throw new Exception("Failed to create game state");
            }

            // Return crash point to client - client will animate and decide when to cash out
            return new CrashGameResultDto
            {
                CrashPoint = crashPoint,
                IsWin = false,
                Payout = 0
            };
        }


        // Cash out at current multiplier
        public async Task<CrashGameResultDto> CashOut(int userAccountId, double multiplierStoppedAt)
        {
            // Get stored game state from singleton service
            if (!_stateService.TryGetGame(userAccountId, out var game) || game == null)
            {
                throw new Exception("No active game found");
            }

            // Remove game from state
            _stateService.RemoveGame(userAccountId);

            // Check if already crashed
            if (multiplierStoppedAt >= game.CrashPoint)
            {
                // Too late - already crashed, no refund
                return new CrashGameResultDto
                {
                    CrashPoint = game.CrashPoint,
                    IsWin = false,
                    Payout = -game.BetAmount
                };
            }

            // Calculate winnings
            int winnings = (int)Math.Floor(game.BetAmount * multiplierStoppedAt);
            
            // Credit user balance
            await _userRepository.UpdateBalanceByIdAsync(userAccountId, winnings);

            // Create win transaction
            var winTransaction = new Transaction
            {
                UserAccountId = userAccountId,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Amount = winnings,
                GameName = "Crash"
            };

            await _transactionRepository.AddAsync(winTransaction);
            await _transactionRepository.SaveChangesAsync();

            // Net payout (winnings - bet)
            int netPayout = winnings - game.BetAmount;

            return new CrashGameResultDto
            {
                CrashPoint = game.CrashPoint,
                IsWin = true,
                Payout = netPayout
            };
        }
    }
}
