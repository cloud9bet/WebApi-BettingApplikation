using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BettingApi.Models;


namespace BettingApi.Data
{
    public static class TestBetDbSeeder
    {
        public async static Task Seed(BetAppDbContext context, UserManager<ApiUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            if (await userManager.FindByNameAsync("admin") != null) return;

            // Create roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminUser = new ApiUser
            {
                UserName = "admin",
            };

            if (await userManager.FindByNameAsync("admin") == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin123456!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var testUserAccount = new UserAccount
            {
                UserName = "user",
                Balance = 10000
            };

            var UserAccountNr1 = new UserAccount
            {
                UserName = "Andreas",
                Balance = 5000
            };

            var UserAccountNr2 = new UserAccount
            {
                UserName = "Niclas",
                Balance = 5000
            };

            var UserAccountNr3 = new UserAccount
            {
                UserName = "Morty",
                Balance = 5000
            };

            var UserAccountNr4 = new UserAccount
            {
                UserName = "Pershy",
                Balance = 5000
            };

            var UserAccountNr5 = new UserAccount
            {
                UserName = "Yu",
                Balance = 5000
            };

            var UserAccountNr6 = new UserAccount
            {
                UserName = "Zak",
                Balance = 5000
            };

            var UserAccountNr7 = new UserAccount
            {
                UserName = "Malle",
                Balance = 5000
            };

            var UserAccountNr8 = new UserAccount
            {
                UserName = "Hiru",
                Balance = 5000
            };

            var UserAccountNr9 = new UserAccount
            {
                UserName = "Joe",
                Balance = 5000
            };

            List<UserAccount> accounts = new List<UserAccount>{testUserAccount,UserAccountNr1,UserAccountNr2,UserAccountNr3,
            UserAccountNr4,UserAccountNr5,UserAccountNr6,UserAccountNr7,UserAccountNr8,UserAccountNr9};

            foreach (var acc in accounts)
            {
                await context.UserAccounts.AddAsync(acc);
            }

            await context.SaveChangesAsync();

            // Alle konti laver 4 deposits á 2500
            foreach (var acc in accounts)
            {
                for (int i = 0; i < 4; i++)
                {
                    var dep = new Deposit
                    {
                        Amount = 2500,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow),
                        UserAccount = acc
                    };

                    await context.Deposits.AddAsync(dep);
                }
            }
            await context.SaveChangesAsync();

            // Halvdelen af kontiene (index 0 og op til 5) taber deres spil
            for (int i = 0; i < 5; i++)
            {
                var acc = accounts[i];

                string[] games = { "Crash", "CoinFlip", "Slots" };

                foreach (var game in games)
                {
                    // 2 transaktioner per spil
                    for (int j = 0; j < 2; j++)
                    {
                        var trans = new Transaction
                        {
                            Date = DateOnly.FromDateTime(DateTime.UtcNow),
                            Amount = -2500,
                            GameName = game,
                            UserAccount = acc
                        };

                        await context.Transactions.AddAsync(trans);
                    }
                }
            }

            // Halvdelen af kontiene (index 5 og op) vinder deres spil
            for (int i = 5; i < accounts.Count; i++)
            {
                var acc = accounts[i];

                string[] games = { "Crash", "CoinFlip", "Slots" };

                foreach (var game in games)
                {
                    // 2 transaktioner per spil
                    for (int j = 0; j < 2; j++)
                    {
                        var trans = new Transaction
                        {
                            Date = DateOnly.FromDateTime(DateTime.UtcNow),
                            Amount = 2500,
                            GameName = game,
                            UserAccount = acc
                        };

                        await context.Transactions.AddAsync(trans);
                    }
                }
            }


            List<ApiUser> users = new List<ApiUser>();

            foreach (var acc in accounts)
            {
                var user = new ApiUser
                {
                    UserName = acc.UserName,
                    UserAccount = acc
                };
                users.Add(user);
            }

            foreach (var u in users)
            {
                if (await userManager.FindByNameAsync(u.UserName) == null)
                {
                    var result = await userManager.CreateAsync(u, "Password123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(u, "User");
                    }
                }
            }

        }
    }

}