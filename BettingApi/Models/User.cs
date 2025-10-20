namespace BettingApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Balance { get; set; }
        public int DepositLimit { get; set; } 



        // 1:N relation til Transaction
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
