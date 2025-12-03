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
                UserName = "User",
                Balance = 50000
            };

            var UserAccountNr1 = new UserAccount
            {
                UserName = "Frosty",
                Balance = 50000
            };

            var UserAccountNr2 = new UserAccount
            {
                UserName = "Niclas",
                Balance = 50000
            };

            var UserAccountNr3 = new UserAccount
            {
                UserName = "Morty",
                Balance = 50000
            };

            var UserAccountNr4 = new UserAccount
            {
                UserName = "Pershy",
                Balance = 50000
            };

            var UserAccountNr5 = new UserAccount
            {
                UserName = "Yu",
                Balance = 50000
            };

            var UserAccountNr6 = new UserAccount
            {
                UserName = "Zak",
                Balance = 50000
            };

            var UserAccountNr7 = new UserAccount
            {
                UserName = "Bal",
                Balance = 50000
            };

            var UserAccountNr8 = new UserAccount
            {
                UserName = "Hiru",
                Balance = 50000
            };

            var UserAccountNr9 = new UserAccount
            {
                UserName = "Joe",
                Balance = 50000
            };

            List<UserAccount> accounts = new List<UserAccount>{testUserAccount,UserAccountNr1,UserAccountNr2,UserAccountNr3,
            UserAccountNr4,UserAccountNr5,UserAccountNr6,UserAccountNr7,UserAccountNr8,UserAccountNr9};

            foreach (var acc in accounts)
            {
                await context.UserAccounts.AddAsync(acc);
            }

            await context.SaveChangesAsync();

            Random rand = new Random();

            int[] days = { 1, 2, 3, 4, 5, 6, 7 };
            string[] games = { "Crash", "CoinFlip", "Slot" };


            foreach (var acc in accounts)
            {
                foreach (var d in days)
                {

                    var dep = new Deposit
                    {
                        Amount = 2000,
                        Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-d)),
                        UserAccount = acc
                    };

                    acc.Balance += 2000;
                    await context.Deposits.AddAsync(dep);
                }
            }

            await context.SaveChangesAsync();


            // TABER-GRUPPE (index 0–4)
            for (int i = 0; i < 5; i++)
            {
                var acc = accounts[i];

                foreach (var d in days)
                {
                    foreach (var game in games)
                    {
                        // 70% chance for et tab, 30% chance for intet
                        if (rand.NextDouble() < 0.7)
                        {
                            int lossAmount = rand.Next(300, 3001); 

                            var trans = new Transaction
                            {
                                Amount = -lossAmount,
                                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-d)),
                                GameName = game,
                                UserAccount = acc
                            };

                            acc.Balance -= lossAmount;
                            await context.Transactions.AddAsync(trans);
                        }
                    }
                }
            }



            // VINDER-GRUPPE (index 5 og op)
            for (int i = 5; i < accounts.Count; i++)
            {
                var acc = accounts[i];

                foreach (var d in days)
                {
                    foreach (var game in games)
                    {
                        // 60% chance for en gevinst
                        if (rand.NextDouble() < 0.8)
                        {
                            int winAmount = rand.Next(100, 1501); 

                            var trans = new Transaction
                            {
                                Amount = winAmount,
                                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-d)),
                                GameName = game,
                                UserAccount = acc
                            };

                            acc.Balance += winAmount;
                            await context.Transactions.AddAsync(trans);
                        }
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