using Microsoft.AspNetCore.Mvc;
using BettingApi.Dto;
using BettingApi.Services;



namespace BettingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto dto)
        {
            await _authService.Register(dto);

            return Ok();

        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.Login(dto);
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound("Account not found");
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout(string refreshToken)
        {
            await _authService.LogOut(refreshToken);
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenDto>> Refresh(string refreshToken)
        {
            var result = await _authService.RefreshJWTToken(refreshToken);
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound("RefreshToken not found");
        }

    }

}
