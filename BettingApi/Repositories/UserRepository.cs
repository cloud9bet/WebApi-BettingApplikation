using BettingApi.Models;
using BettingApi.Dto;
using BettingApi.Data;
using Microsoft.EntityFrameworkCore;
namespace BettingApi.Repositories;

public interface IUserRepository : IRepository<UserAccount>
{
    Task SetDepositLimitByIdAsync(int id, int depositLimit);
    Task UpdateBalanceByIdAsync(int id, int amount);
    Task SetActiveStatusByIdAsync(int id, bool activeStatus);
    Task DeleteUserByIdAsync(int id);
    Task<IEnumerable<UserTagDto>> GetAllUserTagsAsync();
    Task<UserInfoDto> GetAllUserInfoByIdAsync(int id);
    Task<UserPresetsDto> GetUserPresetsByIdAsync(int id);

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


    public async Task<IEnumerable<UserTagDto>> GetAllUserTagsAsync()
    {
        var users = await _dbSet
       .Select(UT => new UserTagDto
       {
           UserAccountId = UT.UserAccountId,
           UserName = UT.UserName
       }).ToListAsync();

        return users;
    }


    public async Task<UserInfoDto> GetAllUserInfoByIdAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {

            return new UserInfoDto
            {
                UserAccountId = user.UserAccountId,
                UserName = user.UserName,
                Balance = user.Balance,
                DepositLimit = user.DepositLimit,
                ActiveStatus = user.ActiveStatus
            };
        }
        return null;
    }


    public async Task<UserPresetsDto> GetUserPresetsByIdAsync(int id)
    {
        var user = await GetByIdAsync(id);
       
            return new UserPresetsDto
            {
                Balance = user.Balance,
                DepositLimit = user.DepositLimit,
                ActiveStatus = user.ActiveStatus
            };
    }
}
