using System.Security.Cryptography;
using BettingApi.Dto;
using BettingApi.Models;
using BettingApi.Repositories;

namespace BettingApi.Services;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllTransactionByUserIdAsync(int id);
    Task AddTrasactionAsync(Transaction transaction);
    Task UpdateGameTransactionByIdAsync(int id, int amount);
    Task<IEnumerable<Transaction>> GetTransactionByGameNameAsync(int id, string GameName, DateOnly date);

    Task<IEnumerable<TransactionDto>> GetAllTransactionAsync();

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

    public async Task AddTrasactionAsync(Transaction transaction)
    {
        await _repository.AddAsync(transaction);
        await _repository.SaveChangesAsync();
    }
    public async Task UpdateGameTransactionByIdAsync(int id, int amount)
    {
        await _repository.UpdateGameTransactionByIdAsync(id, amount);
    }
    public async Task<IEnumerable<Transaction>> GetTransactionByGameNameAsync(int id, string GameName, DateOnly date)
    {
        var result = await _repository.GetTransactionByGameNameAsync(id, GameName, date);
        return result;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionAsync()
    {
        var transactions = await _repository.GetAllTransactionForAllUserAsync();
        return transactions;
    }

}