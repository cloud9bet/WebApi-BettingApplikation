namespace BettingApi.Models
{
    public class Deposit
    {
        public int DepositId { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }

        public int UserAccountId { get; set; }
        public UserAccount UserAccount { get; set; }
    }
}
