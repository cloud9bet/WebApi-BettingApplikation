using BettingApi.Dto;
using BettingApi.Models;
using BettingApi.Repositories;

namespace BettingApi.Services
{

    public interface ICrashGameService
    {
        Task<CrashGameResultDto> CrashGamePlay(CrashGameRequestDto dto, int userAccountId);
        Task<CrashGameResultDto> CashOut(int userAccountId);
    }

    public class CrashGameService : ICrashGameService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly Random _rng = new Random();

        public CrashGameService(
            ITransactionRepository transactionRepository,
            IUserRepository userRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
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

                if (dto.BetAmount > user.Balance)
                {
                    throw new Exception("Insufficient balance");
                }

                DateOnly dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

                // Withdraw bet
                await _userRepository.UpdateBalanceByIdAsync(userAccountId, -dto.BetAmount);

                // Backend generates crash
                double crashPoint = GenerateCrashPoint();

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
                    Date = dateNow,
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


        public async Task<CrashGameResultDto> CashOut(int userAccountId)
        {
            var user = await _userRepository.GetByIdAsync(userAccountId);
            if (user == null || !user.ActiveStatus)
                throw new Exception("User not found or inactive");

            // Simpel demo: bare giv spilleren lidt gevinst
            var random = GenerateCrashPoint();
            double currentMultiplier = 4;
            int payout = (int)(100 * currentMultiplier); // fx 100$ bet som test

            await _userRepository.UpdateBalanceByIdAsync(userAccountId, payout);

            return new CrashGameResultDto
            {
                CrashPoint = currentMultiplier,
                IsWin = true,
                Payout = payout
            };
        }



        // Same crash curve concept used in many crash games (Hopefully)
        private double GenerateCrashPoint()
        {
                double bias = 1.0;
                double r = _rng.NextDouble();
                double val = -Math.Log(1 - r) / bias;
                double rounded = Math.Round(val * 10) / 10;
                return Math.Max(1.03, rounded);
        }
    }
}