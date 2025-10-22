
using BettingApi.Dto;
namespace BettingApi.Services;

public interface IGameService
{   
    bool CoinResultHelper();
    Task<CoinFlipResultDto> CoinFlipGamePlay(CoinFlipRequestDto dto);
}

public class GameService : IGameService
{
    IUserService _userService;
    GameService(IUserService userService)
    {
        _userService = userService;
    }

    private bool CoinResultHelper()
    {
        return true;//fiks efter
    }
    public async Task<CoinFlipResultDto> CoinFlipGamePlay(CoinFlipRequestDto dto)
    {
        var user = await _userService.GetByIdAsync(dto.Id);
        if (user != null)
        {
            if (dto.BetAmount <= user.Balance)
            {
                await _userService.UpdateBalanceByIdAsync(dto.Id, -dto.BetAmount);
    
            //spil logic med random

            var result = CoinResultHelper();

            var resultDto = new CoinFlipResultDto
            {
                Won = result,
                Payout = -dto.BetAmount
            };

            if (dto.Choice == result)
            {
                await _userService.UpdateBalanceByIdAsync(dto.Id, dto.BetAmount*2);
                resultDto.Won = true;
            }

            resultDto.Won = false;

            }
        }
        return resultDto;
    }


}