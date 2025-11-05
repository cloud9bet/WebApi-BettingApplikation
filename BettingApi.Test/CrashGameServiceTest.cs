using Xunit;
using NSubstitute;
using BettingApi.Services;
using BettingApi.Repositories;
using BettingApi.Models;
using BettingApi.Dto;
using System;
using System.Threading.Tasks;


public class CrashGameServiceTests
{
    private readonly CrashGameService _uut;
    private readonly IUserRepository _userRepo;
    private readonly ITransactionRepository _transactionRepo;
    private readonly ICrashRng _rng;

    public CrashGameServiceTests()
    {
        _userRepo = Substitute.For<IUserRepository>();
        _transactionRepo = Substitute.For<ITransactionRepository>();
        _rng = Substitute.For<ICrashRng>();

        _uut = new CrashGameService(_transactionRepo, _userRepo, _rng);
    }


    // ---------- Exception / fejl scenarier ----------

    [Fact]
    public async Task CrashGamePlay_ShouldThrow_WhenUserNotFound()
    {
        _userRepo.GetByIdAsync(1).Returns((UserAccount)null);

        var request = new CrashGameRequestDto
        {
            BetAmount = 100,
            CashoutMultiplier = 1.5
        };

        var ex = await Assert.ThrowsAsync<Exception>(() => _uut.CrashGamePlay(request, 1));
        Assert.Contains("User not active", ex.Message);
    }


    [Fact]
    public async Task CrashGamePlay_ShouldThrow_WhenUserInactive()
    {
        _userRepo.GetByIdAsync(1).Returns(new UserAccount
        {
            UserAccountId = 1,
            Balance = 500,
            ActiveStatus = false
        });

        var request = new CrashGameRequestDto
        {
            BetAmount = 50,
            CashoutMultiplier = 2
        };

        var ex = await Assert.ThrowsAsync<Exception>(() => _uut.CrashGamePlay(request, 1));
        Assert.Contains("User not active", ex.Message);
    }


    [Theory]   // boundary testing
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(501)]
    public async Task CrashGamePlay_BetAmountBoundaryTests_ShouldThrow(int amount)
    {
        _userRepo.GetByIdAsync(1).Returns(new UserAccount
        {
            UserAccountId = 1,
            Balance = 500,
            ActiveStatus = true
        });

        var request = new CrashGameRequestDto
        {
            BetAmount = amount,
            CashoutMultiplier = 1.5
        };

        var ex = await Assert.ThrowsAsync<Exception>(() => _uut.CrashGamePlay(request, 1));
        Assert.Contains("Insufficient balance", ex.Message);
    }


    // ---------- Result scenarier ----------

    [Fact]
    public async Task CrashGamePlay_ShouldReturnWin_WhenCashoutIsBeforeCrash()
    {
        _userRepo.GetByIdAsync(1).Returns(new UserAccount
        {
            UserAccountId = 1,
            Balance = 500,
            ActiveStatus = true
        });

        _rng.Generate().Returns(4.0);   // backend crash at 4.0

        var request = new CrashGameRequestDto
        {
            BetAmount = 100,
            CashoutMultiplier = 2.0
        };

        var result = await _uut.CrashGamePlay(request, 1);

        Assert.True(result.IsWin);
        Assert.Equal(100, result.Payout);        // -100 + 200 = +100
        Assert.Equal(4.0, result.CrashPoint);
    }


    [Fact]
    public async Task CrashGamePlay_ShouldReturnLoss_WhenCrashBeforeCashout()
    {
        _userRepo.GetByIdAsync(1).Returns(new UserAccount
        {
            UserAccountId = 1,
            Balance = 500,
            ActiveStatus = true
        });

        _rng.Generate().Returns(1.2);   // crash early

        var request = new CrashGameRequestDto
        {
            BetAmount = 100,
            CashoutMultiplier = 3.0
        };

        var result = await _uut.CrashGamePlay(request, 1);

        Assert.False(result.IsWin);
        Assert.Equal(-100, result.Payout);
    }


    [Fact]
    public async Task CrashGamePlay_ShouldAddTransaction_WhenGamePlayed()
    {
        _userRepo.GetByIdAsync(1).Returns(new UserAccount
        {
            UserAccountId = 1,
            Balance = 500,
            ActiveStatus = true
        });

        _rng.Generate().Returns(3.0);

        var request = new CrashGameRequestDto
        {
            BetAmount = 50,
            CashoutMultiplier = 2.0
        };

        _ = await _uut.CrashGamePlay(request, 1);

        await _transactionRepo.Received(1).AddAsync(Arg.Any<Transaction>());
    }
}
