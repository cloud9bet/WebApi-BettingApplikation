using BettingApi.Models;
using Microsoft.EntityFrameworkCore;
using BettingApi.Data;
using BettingApi.Dto;

namespace BettingApi.Repositories;
public interface ITransactionRepository:IRepository<Transaction>{
Task<IEnumerable<TransactionDto>> GetAllTransactionByUseridAsync(int id);

}


public class TransactionRepository: Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(BetAppDbContext context) : base(context)
    {

    }
    public async Task<IEnumerable<TransactionDto>> GetAllTransactionByUseridAsync(int id)
    {
         var transactions = await _dbSet.Where(d => d.UserId == id)
        .Select(Dt => new TransactionDto
        {
            Date = Dt.Date,
            Amount = Dt.Amount,
            GameName = Dt.GameName

        }).ToListAsync();

        return transactions;
    }



    
}
