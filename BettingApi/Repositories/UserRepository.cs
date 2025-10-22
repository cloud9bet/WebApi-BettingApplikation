using BettingApi.Models;
using Microsoft.EntityFrameworkCore;
using BettingApi.Data;

namespace BettingApi.Repositories;

public interface IUserRepository : IRepository<UserAccount>
{
    Task SetDepositLimitByIdAsync(int id, int depositLimit);
    Task UpdateBalanceByIdAsync(int id, int amount);
    Task SetActiveStatusByIdAsync(int id, bool activeStatus);
    Task DeleteUserByIdAsync(int id);
}


public class UserRepository : Repository<UserAccount>, IUserRepository
{
    public UserRepository(BetAppDbContext context) : base(context)
    {

    }

    public async Task UpdateBalanceByIdAsync(int id, int amount)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.Balance += amount;
            await SaveChangesAsync();
        }

    }

    public async Task SetDepositLimitByIdAsync(int id, int depositLimit)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.DepositLimit = depositLimit;
            await SaveChangesAsync();
        }
    }
    public async Task SetActiveStatusByIdAsync(int id, bool activeStatus)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.ActiveStatus = activeStatus;
            await SaveChangesAsync();
        }
    }

    public async Task DeleteUserByIdAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            Delete(user);
            await SaveChangesAsync();
        }
    }
}
