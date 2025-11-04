using Microsoft.AspNetCore.Mvc;
using BettingApi.Models;
using BettingApi.Dto;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BettingApi.Services;
using BettingApi.Repositories;

namespace BettingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ICoinFlipService _coinFlipService;

        private readonly ICrashGameService _crashGameService;

        private readonly UserManager<ApiUser> _userManager;

        public GameController(ICoinFlipService coinFlipService, UserManager<ApiUser> userManager, ICrashGameService crashGameService)
        {
            _coinFlipService = coinFlipService;
            _crashGameService = crashGameService;
            _userManager = userManager;
        }


        [Authorize(Roles = "User")]
        [HttpPost("coinflip")]
        public async Task<ActionResult<CoinFlipResultDto>> PlayCoinFlip(CoinFlipRequestDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var userAccountId = user.UserAccountId ?? 0;

                var result = await _coinFlipService.CoinFlipGamePlay(dto, userAccountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Fanger exception fra service-laget
                return BadRequest(new { message = ex.Message });
            }
        }





        [Authorize(Roles = "User")]
        [HttpPost("crash")]
        public async Task<ActionResult<CrashGameResultDto>> PlayCrash(CrashGameRequestDto dto)
        {
            try
            {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            var userAccountId = user.UserAccountId ?? 0;

            var result = await _crashGameService.CrashGamePlay(dto, userAccountId);
            return Ok(result);
            }
            catch (Exception ex)
            {
            return BadRequest(new { message = ex.Message });
            }
        }



    }
}





