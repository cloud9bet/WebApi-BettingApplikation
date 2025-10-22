namespace BettingApi.Dto
{
    public class CoinFlipResultDto
    {
        public String? Result { get; set; } = null;
        public bool? Won { get; set; } = null;
        public int? Payout { get; set; } = null;

    }
}
