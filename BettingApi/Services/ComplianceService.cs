
using BettingApi.Repositories;
using BettingApi.Services;
namespace BettingApi.Services;
public interface IComplianceService
{

}

public class ComplianceService : IComplianceService
{
    private readonly IDepositService _depositService;
    private readonly ITransactionService _transactionService;

    public ComplianceService(IDepositService depositService, ITransactionService transactionService)
    {
        _depositService = depositService;
        _transactionService = transactionService;
    }

}