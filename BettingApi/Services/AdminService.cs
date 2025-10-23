using BettingApi.Dto;
using BettingApi.Models;

namespace BettingApi.Services;

public interface IAdminService
{
    Task SetActiveStatusByIdAsync(int id, bool activeStatus);
    Task<IEnumerable<TransactionDto>> GetAllTransactionByUserIdAsync(int id);

    Task<IEnumerable<TransactionDto>> GetAllTransactionForAllUsersAsync();

    Task<IEnumerable<DepositResultDto>> GetAllDepositByUserIdAsync(int id);

    Task<IEnumerable<DepositResultDto>> GetAllDepositForAllUsersAsync();

}

public class AdminService : IAdminService
{
    private readonly IDepositService _depositService;
    private readonly ITransactionService _transactionService;
    private readonly IUserService _userService;


    public AdminService(IDepositService depositService, ITransactionService transactionService, IUserService userService)
    {
        _depositService = depositService;
        _transactionService = transactionService;
        _userService = userService;

    }

    public async Task SetActiveStatusByIdAsync(int id, bool activeStatus)
    {
        await _userService.SetActiveStatusByIdAsync(id,activeStatus);
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionByUserIdAsync(int id)
    {
        var result = await _transactionService.GetAllTransactionByUserIdAsync(id);
        return result;

    }
    
    public async Task<IEnumerable<TransactionDto>> GetAllTransactionForAllUsersAsync()
    {
        var result = await _transactionService.GetAllTransactionAsync();
        return result;
    }

    public async Task<IEnumerable<DepositResultDto>> GetAllDepositByUserIdAsync(int id)
    {
        var result = await _depositService.GetAllDepositByUserIdAsync(id);
        return result;

    }

    public async Task<IEnumerable<DepositResultDto>> GetAllDepositForAllUsersAsync()
    {
        var result = await _depositService.GetAllDepositAsync();
        return result;
    }


    
    

    

}