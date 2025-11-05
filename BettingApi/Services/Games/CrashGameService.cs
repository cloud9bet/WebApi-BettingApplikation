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
            // Lower values more likely — typical crash game RNG
            return Math.Max(1.0, -Math.Log(_rng.NextDouble()) + 1.0);
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
            ICrashRng rng
        )
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _rng = rng;
        }


        public async Task<CrashGameResultDto> CrashGamePlay(CrashGameRequestDto dto, int userAccountId)
        {
            using var dbOperation = await _userRepository.BeginTransactionAsync();

            try
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

                // Withdraw bet
                await _userRepository.UpdateBalanceByIdAsync(userAccountId, -dto.BetAmount);

                double crashPoint = _rng.Generate();

                bool isWin = dto.CashoutMultiplier < crashPoint;
                int payout = -dto.BetAmount;

                if (isWin)
                {
                    int winnings = (int)(dto.BetAmount * dto.CashoutMultiplier);
                    payout += winnings;

                    await _userRepository.UpdateBalanceByIdAsync(userAccountId, winnings);
                }

                var transaction = new Transaction
                {
                    UserAccountId = userAccountId,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow),
                    Amount = payout,
                    GameName = "Crash"
                };

                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();

                await dbOperation.CommitAsync();

                return new CrashGameResultDto
                {
                    CrashPoint = crashPoint,
                    IsWin = isWin,
                    Payout = payout
                };
            }
            catch (Exception ex)
            {
                await dbOperation.RollbackAsync();
                throw new Exception($"Crash transaction failed: {ex.Message}");
            }
        }
    }
}
