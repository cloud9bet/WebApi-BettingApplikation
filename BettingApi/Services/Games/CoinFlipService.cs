using BettingApi.Dto;
using BettingApi.Models;
using BettingApi.Repositories;

namespace BettingApi.Services;

public interface ICoinFlipService
{
    string GetCoinResult();
    Task<CoinFlipResultDto> CoinFlipGamePlay(CoinFlipRequestDto dto, int id);
}


public class CoinFlipService : ICoinFlipService
{
    private readonly Random _rand = new Random();
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;

    public CoinFlipService(ITransactionRepository transactionRepository, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
    }

    public string GetCoinResult() => _rand.Next(0, 2) == 0 ? "heads" : "tails";


    public async Task<CoinFlipResultDto> CoinFlipGamePlay(CoinFlipRequestDto dto, int id)
    {
        using var dbOperation = await _userRepository.BeginTransactionAsync();

        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null && user.ActiveStatus)
            {

                if (dto.BetAmount <= user.Balance)
                {
                    DateOnly dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

                    await _userRepository.UpdateBalanceByIdAsync(id, -dto.BetAmount);

                    var result = GetCoinResult();

                    var resultDto = new CoinFlipResultDto
                    {
                        Result = result,
                        Payout = -dto.BetAmount
                    };


                    if (dto.Choice == result)
                    {
                        await _userRepository.UpdateBalanceByIdAsync(id, dto.BetAmount * 2);
                        resultDto.Payout += dto.BetAmount * 2;
                    }

                    // var transactions = await _transactionRepository.GetTransactionByGameNameAsync(id, "Coin Flip", dateNow);

                    // if (transactions.Any())
                    // {
                    //     foreach (var transaction in transactions)
                    //     {
                    //         await _transactionRepository.UpdateGameTransactionByIdAsync(transaction.TransactionId, resultDto.Payout);
                    //     }

                    // }
                    // else
                    // {
                    var Transaction = new Transaction
                    {
                        UserAccountId = id,
                        Date = dateNow,
                        Amount = resultDto.Payout,
                        GameName = "Coin Flip"
                    };

                    await _transactionRepository.AddAsync(Transaction);
                    await _transactionRepository.SaveChangesAsync();
                    // }

                    await dbOperation.CommitAsync();
                    return resultDto;
                }

            }
            throw new Exception("User not active from service");
        }
        catch (Exception ex)
        {
            // Rollback hvis noget fejler
            await dbOperation.RollbackAsync();
            throw new Exception($"Coin flip transaction failed: {ex.Message}");
        }
    }
}



