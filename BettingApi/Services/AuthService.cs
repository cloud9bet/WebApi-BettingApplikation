using BettingApi.Models;
using BettingApi.Dto;

//Til Auth
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace BettingApi.Services;

public interface IAuthService
{
    Task Register(RegisterDto dto);
    Task<LoginResultDto> Login(LoginDto dto);

    // Task Refresh(); //laves efter

}

public class AuthService : IAuthService
{

    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApiUser> _userManager;
    AuthService(IUserService userService, IConfiguration configuration, UserManager<ApiUser> userManager)
    {
        _userService = userService;
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<string> getJWT(ApiUser user)
    {
        var signingCredentials = new SigningCredentials(
        new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"])), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        claims.AddRange((await _userManager.GetRolesAsync(user))
        .Select(r => new Claim(ClaimTypes.Role, r)));

        var jwtObject = new JwtSecurityToken(
        issuer: _configuration["JWT:Issuer"],
        audience: _configuration["JWT:Audience"],
        claims: claims,
        expires: DateTime.Now.AddSeconds(300),
        signingCredentials: signingCredentials);

        var jwtString = new JwtSecurityTokenHandler()
        .WriteToken(jwtObject);

        return jwtString;
    }

    public async Task Register(RegisterDto dto)
    {
        var UserAccount = new UserAccount
        {
            UserName = dto.UserName,
            ActiveStatus = true
        };

        await _userService.AddUserAsync(UserAccount);

        var newUser = new ApiUser
        {
            UserName = UserAccount.UserName,
            UserAccount = UserAccount
        };

        var result = await _userManager.CreateAsync(newUser, dto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, "User");
        }
    }

    public async Task<LoginResultDto> Login(LoginDto dto)
    {
        var token = "";

        var Result = new LoginResultDto();


        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            token = await getJWT(user);

            Result.JWTtoken = token;
            Result.RefreshToken = "";
        }
        return Result;
    }

    

    // string RefreshJWTToken(string refreshToken)
    // {
    // }

}


