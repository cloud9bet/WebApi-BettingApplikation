using BettingApi.Models;
using BettingApi.Dto;
using BettingApi.Repositories;



//Til Auth
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace BettingApi.Services;

public interface IAuthService
{
    Task Register(RegisterDto dto);
    Task<LoginResultDto> Login(LoginDto dto);

    // Task Refresh(); //laves efter

}

public class AuthService : IAuthService
{

    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    AuthService(IUserRepository userRepository, IConfiguration configuration, UserManager<ApiUser> userManager, 
    IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
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

        await _userRepository.AddAsync(UserAccount);

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
        var Token = "";

        var Result = new LoginResultDto();


        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            Token = await getJWT(user);
            
            var refresh = new RefreshToken
            {
                Token = CreateRefreshToken(),
                ExpirationDate = DateTime.UtcNow.AddMinutes(5),
                ApiUserId = user.Id,
                

            };

            _refreshTokenRepository.AddAsync(refresh);
            _refreshTokenRepository.SaveChangesAsync();

            Result.JWTtoken = Token;
            Result.RefreshToken = refresh.Token ;
        }
        return Result;
    }

    

    public async Task RefreshJWTToken(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetRefreshTokenByValue(refreshToken);
        
        if(token != null)
        {
            await _refreshTokenRepository.UpdateRefreshToken(token.RefreshId,DateTime.UtcNow.AddMinutes(5),CreateRefreshToken());

        }
        
        
    }

    public string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

}


