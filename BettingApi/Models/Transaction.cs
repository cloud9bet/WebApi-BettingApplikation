
namespace BettingApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public string GameName {get; set;}

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
