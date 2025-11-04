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

        private readonly UserManager<ApiUser> _userManager;

        public GameController(ICoinFlipService coinFlipService, UserManager<ApiUser> userManager)
        {
            _coinFlipService = coinFlipService;
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


    }
}





