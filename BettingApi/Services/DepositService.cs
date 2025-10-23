
using BettingApi.Repositories;
using BettingApi.Models;

using BettingApi.Dto;

namespace BettingApi.Services;

public interface IDepositService
{
    Task<IEnumerable<DepositResultDto>> GetAllDepositByUserIdAsync(int id);

    Task<IEnumerable<DepositResultDto>> GetAllDepositAsync();

    Task AddDepositAsync(DepositRequestDto dto);
}

public class DepositService : IDepositService
{
    private readonly IDepositRepository _repository;
    private readonly IUserService _userService;
    DepositService(IDepositRepository repository, IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<IEnumerable<DepositResultDto>> GetAllDepositByUserIdAsync(int id)
    {
        var deposits = await _repository.GetAllDepositByUserIdAsync(id); 

        return deposits;
    }
    
    public async Task AddDepositAsync(DepositRequestDto dto)
    {
        var user = await _userService.GetByIdAsync(dto.Id);
        if (user != null)
        {
            if(user.DepositLimit >= dto.Amount || user.DepositLimit == null)
            {
                var deposit = new Deposit
                {
                    Amount = dto.Amount,
                    Date = new DateOnly(),
                    UserAccountId = dto.Id
                };
                await _repository.AddAsync(deposit);
                await _repository.SaveChangesAsync();
            }
        }
    }

    public async Task<IEnumerable<DepositResultDto>> GetAllDepositAsync()
    {
        var deposits = await _repository.GetAllDepositForAllUsersAsync();
        return deposits; 
    }


}