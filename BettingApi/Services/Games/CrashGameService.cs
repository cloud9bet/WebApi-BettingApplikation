using BettingApi.Dto;
using BettingApi.Models;
using BettingApi.Repositories;

namespace BettingApi.Services
{
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
    }


    public class CrashGameService : ICrashGameService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICrashRng _rng;

        public CrashGameService(
            ITransactionRepository transactionRepository, 
            IUserRepository userRepository, 
            ICrashRng rng)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _rng = rng;
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

            // Return crash point to client - client will animate and decide when to cash out
            return new CrashGameResultDto
            {
                CrashPoint = crashPoint,
                IsWin = false,
                Payout = 0
            };
        }

    }
}
