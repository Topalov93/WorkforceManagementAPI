using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Managers
{
    public interface IUserManager
    {
        Task CreateUserAsync(User user, string password, string role);

        Task<List<User>> GetAllAsync();

        Task<User> FindByNameAsync(string userName);

        Task<bool> ValidateUserCredentials(string userName, string password);

        Task<List<string>> GetUserRolesAsync(User user);

        Task<User> FindByEmailAsync(string email);

        Task<List<IdentityRole>> GetAllRoles();

        Task EditUserById(string userId, User newInfo);

        Task<User> FindByIdAsync(string userId);

        Task DeleteUserBy(string userId);

        Task SetInitialDaysOff(string userId, User initialDaysOffInfo);

        Task DecreaseRemainingDaysOff(string userId, int daysToRemove, RequestType type);

        Task IncreaseRemainingDaysOff(string userId, int daysToAdd, RequestType type);
    }
}
