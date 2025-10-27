using Microsoft.AspNetCore.Mvc;
using BettingApi.Models;
using BettingApi.Repositories;
using BettingApi.Dto;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BettingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly ITransactionRepository _transactionRepository;

        private readonly UserManager<ApiUser> _userManager;

        public AdminController(IUserRepository userRepository, IDepositRepository depositRepository, UserManager<ApiUser> userManager)
        {
            _userRepository = userRepository;
            _depositRepository = depositRepository;
            _userManager = userManager;
        }

        [Authorize(Roles = "User")]
        [HttpPut]
        public async Task<ActionResult> SetUserActiveState(int id, bool status)
        {
            var user = await _userRepository.GetByIdAsync(id);
                
                await _userRepository.SetActiveStatusByIdAsync(id, status);
                return Ok();
            

            // return NotFound();

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/user")]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetAllUserInfoAsync()
        {
            var users = await _userRepository.GetAllUserInfoAsync();

            if (users != null)
            {
                {
                    return Ok(users);
                }
            }
            else
                return NotFound($"Users was not found");

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/deposit")]
        public async Task<ActionResult<IEnumerable<DepositResultDto>>> GetAllUserDepositAsync()
        {
            var deposit = await _depositRepository.GetAllDepositForAllUsersAsync();

            if (deposit != null)
            {
                {
                    return Ok(deposit);
                }
            }
            else
                return NotFound($"No deposit was found");

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/transaction")]
        public async Task<ActionResult<IEnumerable<DepositResultDto>>> GetAllUserTransactionAsync()
        {
            var transaction = await _transactionRepository.GetAllTransactionForAllUserAsync();

            if (transaction != null)
            {
                {
                    return Ok(transaction);
                }
            }
            else
                return NotFound($"No transaction was found");

        }

        //mangler de to metoder for at hente deposit og transactioner for et id
    }
}




