using BettingApi.Dto;
using BettingApi.Repositories;

namespace BettingApi.Services;
public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllTransactionByUserIdAsync(int id);
}

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionByUserIdAsync(int id)
    {
        var transactions = await _repository.GetAllTransactionByUserIdAsync(id);
        return transactions;
    }
}