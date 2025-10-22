
using BettingApi.Repositories;
using BettingApi.Dto;

namespace BettingApi.Services;

public interface IDepositService
{
    Task<IEnumerable<DepositDto>> GetAllDepositByUserIdAsync(int id);

}

public class DepositService : IDepositService
{
    private readonly IDepositRepository _repository;
    DepositService(IDepositRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DepositDto>> GetAllDepositByUserIdAsync(int id)
    {
        var deposits = await _repository.GetAllDepositByUserIdAsync(id);
        return deposits;
    }
   

}