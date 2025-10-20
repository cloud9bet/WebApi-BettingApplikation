// using System;
// using System.Collections.Generic;
// using System.Linq;
// using BettingApi.Models;
// using Microsoft.EntityFrameworkCore;

// namespace BettingApi.Data
// {
//     public static class TestBetDbSeeder
//     {
//         public static void Seed(BetAppDbContext context)
//         {
//             context.Database.EnsureCreated();

//             // Slet eksisterende data
//             context.Transactions.RemoveRange(context.Transactions);
//             context.Users.RemoveRange(context.Users);
//             context.SaveChanges();

//             // Resæt identity
//             context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Users', RESEED, 0)");
//             context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Transactions', RESEED, 0)");

//             if (!context.Users.Any())
//             {
//                 var users = new List<User>
//                 {
//                     new User
//                     {
//                         Username = "Alice",
//                         Password = "password123",
//                         Amount = 1000.0,
//                         Transactions = new List<Transaction>
//                         {
//                             new Transaction { Date = DateTime.Today, Amount = 200.0 },
//                             new Transaction { Date = DateTime.Today.AddDays(-1), Amount = -50.0 },
//                             new Transaction { Date = DateTime.Today.AddDays(-2), Amount = 300.0 }
//                         }
//                     },
//                     new User
//                     {
//                         Username = "Bob",
//                         Password = "secure456",
//                         Amount = 500.0,
//                         Transactions = new List<Transaction>
//                         {
//                             new Transaction { Date = DateTime.Today, Amount = 100.0 },
//                             new Transaction { Date = DateTime.Today.AddDays(-1), Amount = 50.0 }
//                         }
//                     },
//                     new User
//                     {
//                         Username = "Charlie",
//                         Password = "qwerty789",
//                         Amount = 750.0,
//                         Transactions = new List<Transaction>
//                         {
//                             new Transaction { Date = DateTime.Today, Amount = 150.0 },
//                             new Transaction { Date = DateTime.Today.AddDays(-1), Amount = -25.0 },
//                             new Transaction { Date = DateTime.Today.AddDays(-2), Amount = 50.0 },
//                             new Transaction { Date = DateTime.Today.AddDays(-3), Amount = 75.0 }
//                         }
//                     }
//                 };

//                 context.Users.AddRange(users);
//                 context.SaveChanges();
//             }
//         }
//     }
// }
