namespace BettingApi.Services;

public interface IAdminService
{

}

public class AdminService : IAdminService
{
    private readonly IDepositService _depositService;
    private readonly ITransactionService _transactionService;

    public AdminService(IDepositService depositService, ITransactionService transactionService)
    {
        _depositService = depositService;
        _transactionService = transactionService;
    }

}