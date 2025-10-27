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
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly UserManager<ApiUser> _userManager;
        private readonly ITransactionRepository _transactionRepository;

        public UserController(IUserRepository userRepository, IDepositRepository depositRepository, 
        ITransactionRepository transactionRepository, UserManager<ApiUser> userManager)
        {
            _userRepository = userRepository;
            _depositRepository = depositRepository;
            _userManager = userManager;
            _transactionRepository = transactionRepository;
        }

        [Authorize(Roles = "User")]
        [HttpPut]
        public async Task<ActionResult> SetUserDepositLimit(int amount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                await _userRepository.SetDepositLimitByIdAsync(user.UserAccountId ?? 0, amount);
                return Ok();
            }

            return NotFound();

        }

        [Authorize(Roles = "User")]
        [HttpDelete]
        public async Task<ActionResult> DeleteUser(int amount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userRepository.DeleteUserByIdAsync(user.UserAccountId ?? 0);

                await _userManager.DeleteAsync(user);

                return Ok();
            }

            return NotFound();

        }


        [Authorize(Roles = "User")]
        [HttpGet("/deposit")]
        public async Task<ActionResult<IEnumerable<DepositResultDto>>> GetAllUserDepositAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var deposit = await _depositRepository.GetAllDepositByUserIdAsync(user.UserAccountId ?? 0);
                return Ok(deposit);

            }
            else
                return NotFound($"User was not found");

        }

        [Authorize(Roles = "User")]
        [HttpGet("/transaction")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllUserTransactionAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var deposit = await _transactionRepository.GetAllTransactionByUserIdAsync(user.UserAccountId ?? 0);
                return Ok(deposit);

            }
            else
                return NotFound($"User had not found");

        }

    }
}





