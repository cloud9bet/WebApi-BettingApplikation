using BettingApi.Models;
using BettingApi.Dto;
namespace BettingApi.Services;

public interface IGameService
{
    Task<CoinFlipResultDto> CoinFlipGamePlay(CoinFlipRequestDto dto);
}

public class GameService : IGameService
{
    IUserService _userService;
    ITransactionService _transactionService;
    GameService(IUserService userService, ITransactionService transactionService)
    {
        _userService = userService;
        _transactionService = transactionService;
    }


    //--------------------------------For CoinGame-------------------------------//
    private string CoinResultHelper()
    {
        Random rand = new Random();
        return rand.Next(0, 2) == 0 ? "heads" : "tails";
    }
    public async Task<CoinFlipResultDto> CoinFlipGamePlay(CoinFlipRequestDto dto)
    {
        var user = await _userService.GetByIdAsync(dto.Id);
        if (user != null)
        {

            if (dto.BetAmount <= user.Balance)
            {
                DateOnly dateNow = new DateOnly();

                await _userService.UpdateBalanceByIdAsync(dto.Id, -dto.BetAmount);

                var result = CoinResultHelper();

                var resultDto = new CoinFlipResultDto
                {
                    Result = result,
                    Payout = -dto.BetAmount
                };


                if (dto.Choice == result)
                {
                    await _userService.UpdateBalanceByIdAsync(dto.Id, dto.BetAmount * 2);
                    resultDto.Payout = dto.BetAmount;
                }

                var transactions = await _transactionService.GetTransactionByGameNameAsync(dto.Id, "Coin Flip", dateNow);

                if (transactions != null)
                {
                    foreach (var transaction in transactions)
                    {
                        await _transactionService.UpdateGameTransactionByIdAsync(transaction.TransactionId, dto.BetAmount);
                    }
                    
                }
                else
                {
                    var Transaction = new Transaction
                    {
                        Date = dateNow,
                        Amount = -dto.BetAmount,
                        GameName = "Coin Flip"
                    };

                    await _transactionService.AddTrasactionAsync(Transaction);
                }

                return resultDto;
            }
        }

        return null;
    }

    //----------------------------------------------------------------------//

}