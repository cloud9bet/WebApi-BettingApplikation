using Xunit;
using NSubstitute;
using BettingApi.Services;
using BettingApi.Repositories;
using BettingApi.Models;
using BettingApi.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Xml.Serialization;

//malleballe
public class SlotMachineServiceTest
{
    private readonly SlotMachineService _uut;
    private readonly ICalculatePayout _calculatePayout;
    private readonly IGenerateGrid _generateGrid;
    private readonly IUserRepository _userRepo;
    private readonly ITransactionRepository _transactionRepo;

    public SlotMachineServiceTest()
    {
        _userRepo = Substitute.For<IUserRepository>();
        _transactionRepo = Substitute.For<ITransactionRepository>();
        _calculatePayout=Substitute.For<ICalculatePayout>();
        _generateGrid=Substitute.For<IGenerateGrid>();
        _uut=new SlotMachineService(_userRepo,_transactionRepo,_generateGrid,_calculatePayout);
    }

    // ---------- Exception / fejl scenarier ----------
    [Fact]
    public async Task SlotMachineGamePlay_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        _userRepo.GetByIdAsync(1).Returns((UserAccount)null);
        // Act + Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _uut.SlotMachinePlay(100,1));
        Assert.Equal("Slotmachine transaction failed: User not found or inactive", ex.Message);
    }

    [Fact]
    public async Task SlotMachineGamePlay_ShouldThrow_WhenUserInactive()
    {
        // Arrange
        _userRepo.GetByIdAsync(1).Returns(new UserAccount
        {
            UserAccountId = 1,
            UserName = "Hans",
            Balance = 1000,
            ActiveStatus = false
        });

        // Act + Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _uut.SlotMachinePlay(100, 1));
        Assert.Equal("Slotmachine transaction failed: User not found or inactive", ex.Message);
    }

    [Theory]// bva og ep
    [InlineData(-1)]   // negative → invalid
    [InlineData(0)]    // zero → invalid
    [InlineData(1)]    // min valid → valid
    [InlineData(10)]   // max valid → valid
    [InlineData(11)]   // over balance → invalid
    public async Task SlotMachineGamePlay_BetAmountBoundaryTests(int amount)
    {
        _userRepo.GetByIdAsync(1).Returns(new UserAccount
        {
            UserAccountId = 1,
            UserName = "Hans",
            Balance = 10,
            ActiveStatus = true
        });

        if (amount <= 0 || amount > 10)
        {
            var ex = await Assert.ThrowsAsync<Exception>(() => _uut.SlotMachinePlay(amount, 1));
            Assert.Contains("Insufficient funds", ex.Message);
        }
        else
        {
            var result = await _uut.SlotMachinePlay(amount, 1);
            Assert.NotNull(result); //at spillet succeded
        }
    }

    // ---------- Resultat / payout scenarier ----------
    [Fact]
    public async Task SlotMacginePlay_ShouldReturnNegativeResult_WhenNoWin()
    {
        var grid = new[] { new[]{"🍒","🍋","🪙"}, 
                           new[]{"🍀","9️⃣","🍋"}, 
                           new[]{"🪙","🍒","🍀"}};
        
        var user=new UserAccount{UserAccountId=1,Balance=100,ActiveStatus=true};
        _userRepo.GetByIdAsync(1).Returns(user);
        _generateGrid.MakeGrid().Returns(grid);
        _calculatePayout.CalcPayout(grid,10).Returns(0);

        var result=await _uut.SlotMachinePlay(10,1);

        Assert.Equal(-10,result.Payout);
        await _userRepo.Received().UpdateBalanceByIdAsync(1,-10);
        await _userRepo.DidNotReceive().UpdateBalanceByIdAsync(1,10);

    }

    [Fact]
    public async Task SlotMacginePlay_ShouldReturnPositiveResult_WhenWin()
    {
        var grid = new[] { new[]{"🍒","🍋","🪙"}, 
                           new[]{"🍒","9️⃣","🍋"}, 
                           new[]{"🍒","🍒","🍀"}};
        
        var user=new UserAccount{UserAccountId=1,Balance=100,ActiveStatus=true};
        _userRepo.GetByIdAsync(1).Returns(user);
        _generateGrid.MakeGrid().Returns(grid);
        _calculatePayout.CalcPayout(grid,10).Returns(15);

        var result=await _uut.SlotMachinePlay(10,1);

        Assert.Equal(5,result.Payout);
        await _userRepo.Received().UpdateBalanceByIdAsync(1,-10);
        await _userRepo.Received().UpdateBalanceByIdAsync(1,15);
    }
}
