using Xunit;
using NSubstitute;
using BettingApi.Services;
using BettingApi.Repositories;
using BettingApi.Models;
using BettingApi.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DepositServiceTests
{
    private readonly DepositService _uut;
    private readonly IDepositRepository _depositRepo;
    private readonly IUserRepository _userRepo;

    public DepositServiceTests()
    {
        _depositRepo = Substitute.For<IDepositRepository>();
        _userRepo = Substitute.For<IUserRepository>();
        _uut = new DepositService(_depositRepo, _userRepo);
    }



    [Fact]
    public async Task AddDepositAsync_ShouldAddDeposit_WhenWithinLimit()
    {
        // Arrange
        var user = new UserAccount
        {
            UserAccountId = 1,
            UserName = "Hans",
            Balance = 100,
            DepositLimit = 200
        };

        _userRepo.GetByIdAsync(1).Returns(user);

        int amount = 150;

        // Act
        await _uut.AddDepositAsync(amount, 1);

        // Assert
        Assert.Equal(250, user.Balance); // balance opdateret korrekt
    }

    [Fact]
    public async Task AddDepositAsync_ShouldAddDeposit_WhenNoDepositLimit()
    {
        // Arrange
        var user = new UserAccount
        {
            UserAccountId = 1,
            UserName = "Hans",
            Balance = 100,
            DepositLimit = null
        };

        _userRepo.GetByIdAsync(1).Returns(user);

        int amount = 500;

        // Act
        await _uut.AddDepositAsync(amount, 1);

        // Assert
        Assert.Equal(600, user.Balance); 
    }

    // ---------- Hentning af deposits ----------

    [Fact]
    public async Task GetAllDepositByUserIdAsync_ShouldReturnDepositsForUser()
    {
        // Arrange
        var deposits = new List<DepositResultDto>
        {
            new DepositResultDto { Amount = 100 },
            new DepositResultDto { Amount = 200 }
        };

        _depositRepo.GetAllDepositByUserIdAsync(1).Returns(Task.FromResult((IEnumerable<DepositResultDto>)deposits));

        // Act
        var result = await _uut.GetAllDepositByUserIdAsync(1);

        // Assert
        Assert.Equal(2, ((List<DepositResultDto>)result).Count);
    }


}