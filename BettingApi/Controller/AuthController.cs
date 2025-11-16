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
            var result = await _authService.Register(dto);
            if (result)
            {
            return StatusCode(201);
            }
            return BadRequest("Password does not meet the requirements");

        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.Login(dto);
            if (result != null)
            {
                return Ok(result); 
            }  
            return NotFound("Invalid login credentials");
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenDto>> Refresh(string refreshToken)
        {
            var result = await _authService.RefreshJWTToken(refreshToken);
            if (result != null) //dog vil den aldrig være null men empty dto
            {
                return Ok(result);
            }

            return NotFound("RefreshToken not found or expired");
        }

    }

}