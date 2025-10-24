using BettingApi.Models;
using BettingApi.Dto;
using BettingApi.Repositories;

namespace BettingApi.Services;

public interface IGameService
{
    Task<CoinFlipResultDto> CoinFlipGamePlay(CoinFlipRequestDto dto);
}

public class GameService : IGameService
{
    IUserRepository _userRepository;
    ITransactionRepository _transactionRepository;
    GameService(ITransactionRepository transactionRepository, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
    }


    //--------------------------------For CoinGame-------------------------------//
    private string CoinResultHelper()
    {
        Random rand = new Random();
        return rand.Next(0, 2) == 0 ? "heads" : "tails";
    }
    public async Task<CoinFlipResultDto> CoinFlipGamePlay(CoinFlipRequestDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.Id);
        if (user != null)
        {

            if (dto.BetAmount <= user.Balance)
            {
                DateOnly dateNow = new DateOnly();

                await _userRepository.UpdateBalanceByIdAsync(dto.Id, -dto.BetAmount);

                var result = CoinResultHelper();

                var resultDto = new CoinFlipResultDto
                {
                    Result = result,
                    Payout = -dto.BetAmount
                };


                if (dto.Choice == result)
                {
                    await _userRepository.UpdateBalanceByIdAsync(dto.Id, dto.BetAmount * 2);
                    resultDto.Payout += dto.BetAmount*2;
                }

                var transactions = await _transactionRepository.GetTransactionByGameNameAsync(dto.Id, "Coin Flip", dateNow);

                if (transactions != null)
                {
                    foreach (var transaction in transactions)
                    {
                        await _transactionRepository.UpdateGameTransactionByIdAsync(transaction.TransactionId, resultDto.Payout);
                    }
                    
                }
                else
                {
                    var Transaction = new Transaction
                    {
                        UserAccountId = dto.Id,
                        Date = dateNow,
                        Amount = resultDto.Payout,
                        GameName = "Coin Flip"
                    };

                    await _transactionRepository.AddAsync(Transaction);
                }

                return resultDto;
            }
        }

        return null;
    }

    //----------------------------------------------------------------------//

}