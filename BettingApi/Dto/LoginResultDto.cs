namespace BettingApi.Dto
{
    public class LoginResultDto
    {
        //maybe tilføjer flere attributter for undgå ekstra get req fra user info
        public string? JWTtoken { get; set; }
        public string? RefreshToken { get; set; } 
        
    }
}