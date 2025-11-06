using BettingApi.Services;
using Microsoft.AspNetCore.SignalR;

namespace BettingApi.Hubs
{
    public class CrashHub : Hub
    {
        private readonly ICrashGameService _crashGameService;

        public CrashHub(ICrashGameService crashGameService)
        {
            _crashGameService = crashGameService;
        }

        public async Task StartGame(int betAmount, double cashoutMultiplier, int userAccountId)
        {
            try
            {
                var result = await _crashGameService.CrashGamePlay(
                    new Dto.CrashGameRequestDto
                    {
                        BetAmount = betAmount,
                        CashoutMultiplier = cashoutMultiplier
                    },
                    userAccountId
                );

                // Send løbende updates (demo for nu)
                double multiplier = 1.0;
                while (multiplier < result.CrashPoint)
                {
                    multiplier += 0.05;
                    await Clients.Caller.SendAsync("CrashUpdate", multiplier);
                    await Task.Delay(100);
                }

                // Når spillet er færdigt — send resultatet tilbage
                await Clients.Caller.SendAsync("CrashEnd", result);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("CrashError", ex.Message);
            }
        }


        public async Task CashOut(int userAccountId)
        {
            try
            {
                Console.WriteLine($"💰 CashOut called | UserId={userAccountId}");

                var result = await _crashGameService.CashOut(userAccountId);

                // Send resultatet tilbage til spilleren
                await Clients.Caller.SendAsync("CrashCashedOut", result);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("CrashError", ex.Message);
            }
        }

    }
}
