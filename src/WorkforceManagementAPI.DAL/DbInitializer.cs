using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.DAL
{
    public class DbInitializer
    {
        public static void Seed(IServiceProvider applicationServices)
        {
            using (IServiceScope serviceScope = applicationServices.CreateScope())
            {
                WorkforceManagementAPIDbContext context = serviceScope.ServiceProvider.GetRequiredService<WorkforceManagementAPIDbContext>();

                if (context.Database.EnsureCreated())
                {
                    PasswordHasher<User> hasher = new();

                    //Add holidays
                    HashSet<OfficialBulgarianHolidays> holidays = new();
                    string json = File.ReadAllText("OfficialHolidays.json");
                    holidays = JsonSerializer.Deserialize<HashSet<OfficialBulgarianHolidays>>(json);

                    //Demo for weekends
                    static IEnumerable<DateTime> GetDaysBetween(DateTime start, DateTime end)
                    {
                        for (DateTime i = start; i < end; i = i.AddDays(1))
                        {
                            yield return i;
                        }
                    }
                    var weekends = GetDaysBetween(new DateTime(2021, 1, 1), new DateTime(2022, 2, 1))

                        .Where(d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday);

                    foreach (DateTime date in weekends)
                    {
                        OfficialBulgarianHolidays weekend = new() { Holiday = date };
                        holidays.Add(weekend);
                    }
                    context.Holidays.AddRange(holidays);


                    //Add Admin
                    User admin = new()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Email = "admin@test.test",
                        NormalizedEmail = "admin@test.test".ToUpper(),
                        EmailConfirmed = true,
                        UserName = "admin@test.test",
                        NormalizedUserName = "admin@test.test".ToUpper(),
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        IsWorking = true
                    };

                    admin.PasswordHash = hasher.HashPassword(admin, "adminpass");
                    context.Users.Add(admin);

                    //Add Admin Role
                    IdentityRole identityRoleAdmin = new()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Name = "admin",
                        NormalizedName = "Admin".ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString("D")
                    };

                    IdentityUserRole<string> identityUserRoleAdmin = new() { RoleId = identityRoleAdmin.Id, UserId = admin.Id };

                    context.Roles.Add(identityRoleAdmin);
                    context.UserRoles.Add(identityUserRoleAdmin);

                    //Add Regular Role
                    IdentityRole identityRoleRegularUser = new()
                    {
                        Id = Guid.NewGuid().ToString("D"),
                        Name = "regular",
                        NormalizedName = "regular".ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString("D")
                    };

                    context.Roles.Add(identityRoleRegularUser);
                    context.SaveChanges();
                }
            }
        }
    }
}
