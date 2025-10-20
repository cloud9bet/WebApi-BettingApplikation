using BettingApi.Models;
using Microsoft.EntityFrameworkCore;
using BettingApi.Data;
using BettingApi.Dto;

namespace BettingApi.Repositories;
public interface IDepositRepository:IRepository<Deposit>{
Task<IEnumerable<DepositDto>> GetAllDepositByUseridAsync(int id);

}


public class DepositRepository: Repository<Deposit>, IDepositRepository
{
    public DepositRepository(BetAppDbContext context) : base(context)
    {

    }
    public async Task<IEnumerable<DepositDto>> GetAllDepositByUseridAsync(int id)
    {
        var deposits = await _dbSet.Where(d => d.UserId == id)
        .Select(Dt => new DepositDto
        {
            Date = Dt.Date,
            Amount = Dt.Amount
            
        }).ToListAsync();

        return deposits;
    }

    
}
