
using BettingApi.Repositories;
using BettingApi.Models;

namespace BettingApi.Services;

public interface IUserService
{
    Task SetDepositLimitByIdAsync(int id, int depositLimit);
    Task UpdateBalanceByIdAsync(int id, int amount);
    Task SetActiveStatusByIdAsync(int id, bool activeStatus);
    Task DeleteUserByIdAsync(int id);
    Task AddUserAsync(UserAccount user);
    Task<UserAccount> GetByIdAsync(int id);


}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task SetDepositLimitByIdAsync(int id, int depositLimit)
    {
        await _repository.SetDepositLimitByIdAsync(id, depositLimit);
    }
    public async Task UpdateBalanceByIdAsync(int id, int amount)
    {
        await _repository.UpdateBalanceByIdAsync(id, amount);
        
    }
    public async Task SetActiveStatusByIdAsync(int id, bool activeStatus)
    {
        await _repository.SetActiveStatusByIdAsync(id, activeStatus);
        
    }
    public async Task DeleteUserByIdAsync(int id)
    {
        await _repository.DeleteUserByIdAsync(id);

    }
    public async Task AddUserAsync(UserAccount user)
    {
        await _repository.AddAsync(user);
    }
    
    public async Task<UserAccount> GetByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user;
    }


}