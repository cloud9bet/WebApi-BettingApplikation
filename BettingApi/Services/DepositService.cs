
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
    private readonly IDepositRepository _depositrepository;
    private readonly IUserRepository _userRepository;
    DepositService(IDepositRepository depositrepository, IUserRepository userRepository)
    {
        _depositrepository = depositrepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<DepositResultDto>> GetAllDepositByUserIdAsync(int id)
    {
        var deposits = await _depositrepository.GetAllDepositByUserIdAsync(id); 

        return deposits;
    }
    
    public async Task AddDepositAsync(DepositRequestDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.Id);
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
                await _depositrepository.AddAsync(deposit);
                await _depositrepository.SaveChangesAsync();
            }
        }
    }

    public async Task<IEnumerable<DepositResultDto>> GetAllDepositAsync()
    {
        var deposits = await _depositrepository.GetAllDepositForAllUsersAsync();
        return deposits; 
    }


}