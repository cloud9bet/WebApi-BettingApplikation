using BettingApi.Models;
using Microsoft.EntityFrameworkCore;
using BettingApi.Data;
using BettingApi.Dto;

namespace BettingApi.Repositories;
public interface ITransactionRepository:IRepository<Transaction>
{
Task<IEnumerable<TransactionDto>> GetAllTransactionByUserIdAsync(int id);
}

public class TransactionRepository: Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(BetAppDbContext context) : base(context)
    {

    }
    
    public async Task<IEnumerable<TransactionDto>> GetAllTransactionByUserIdAsync(int id)
    {
         var transactions = await _dbSet.Where(d => d.UserAccountId == id)
        .Select(Dt => new TransactionDto
        {
            Date = Dt.Date,
            Amount = Dt.Amount,
            GameName = Dt.GameName

        }).ToListAsync();

        return transactions;
    }

}
