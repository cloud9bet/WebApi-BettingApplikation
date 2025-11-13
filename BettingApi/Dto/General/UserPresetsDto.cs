
namespace BettingApi.Dto
{
    public class UserPresetsDto
    {
        public int Balance { get; set; }
        public int? DepositLimit { get; set; }
        public bool ActiveStatus { get; set; } = true;

    }
}
