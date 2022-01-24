using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.DAL;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Managers
{
    [ExcludeFromCodeCoverage]
    public class WorkforceManagementAPIUserManager : UserManager<User>, IUserManager
    {
        private readonly WorkforceManagementAPIDbContext _dbContext;

        public WorkforceManagementAPIUserManager(
           IUserStore<User> store,
           IOptions<IdentityOptions> optionsAccessor,
           IPasswordHasher<User> passwordHasher,
           IEnumerable<IUserValidator<User>> userValidators,
           IEnumerable<IPasswordValidator<User>> passwordValidators,
           ILookupNormalizer keyNormalizer,
           IdentityErrorDescriber errors,
           IServiceProvider services,
           ILogger<UserManager<User>> logger,
           WorkforceManagementAPIDbContext context) : base(
       store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _dbContext = context;
        }

        public async Task<List<IdentityRole>> GetAllRoles()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task CreateUserAsync(User user, string password, string role)
        {
            await CreateAsync(user, password);

            await AddToRoleAsync(user, role);
        }

        public async Task EditUserById(string userId, User newInfo)
        {
            User userToEdit = await FindByIdAsync(userId);

            userToEdit.UserName = newInfo.Email;
            userToEdit.Email = newInfo.Email;
            userToEdit.ModifiedOn = newInfo.ModifiedOn;

            await UpdateAsync(userToEdit);
        }

        public async Task DeleteUserBy(string userId)
        {
            User userToDelete = await FindByIdAsync(userId);

            userToDelete.IsDeleted = true;
            userToDelete.IsWorking = false;
            userToDelete.EmailConfirmed = false;
            userToDelete.ModifiedOn = DateTime.UtcNow;
            userToDelete.Email = null;
            userToDelete.UserName = "empty";
            userToDelete.PhoneNumber = null;
            userToDelete.PasswordHash = null;
            userToDelete.InitialPaidDaysOff = 0;
            userToDelete.InitialUnpaidDaysOff = 0;
            userToDelete.InitialSickDaysOff = 0;
            userToDelete.RemainingPaidDaysOff = 0;
            userToDelete.RemainingUnpaidDaysOff = 0;
            userToDelete.RemainingSickDaysOff = 0;


            await UpdateAsync(userToDelete);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await Users.ToListAsync();
        }

        public override Task<User> FindByNameAsync(string userName)
        {
            return base.FindByNameAsync(userName);
        }

        public override Task<User> FindByEmailAsync(string email)
        {
            return base.FindByEmailAsync(email);
        }

        public async Task<bool> ValidateUserCredentials(string userName, string password)
        {
            User user = await FindByNameAsync(userName);
            if (user != null)
            {
                bool result = await CheckPasswordAsync(user, password);
                return result;
            }
            return false;
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            return (await GetRolesAsync(user)).ToList();
        }

        public override Task<User> FindByIdAsync(string userId)
        {
            return base.FindByIdAsync(userId);
        }

        public async Task SetInitialDaysOff(string userId, User initialDaysOffInfo)
        {
            User userToSet = await Users.FirstOrDefaultAsync(u => u.Id == userId);

            userToSet.InitialPaidDaysOff = initialDaysOffInfo.InitialPaidDaysOff;
            userToSet.InitialUnpaidDaysOff = initialDaysOffInfo.InitialUnpaidDaysOff;
            userToSet.InitialSickDaysOff = initialDaysOffInfo.InitialSickDaysOff;
            userToSet.RemainingPaidDaysOff = initialDaysOffInfo.InitialPaidDaysOff;
            userToSet.RemainingUnpaidDaysOff = initialDaysOffInfo.InitialUnpaidDaysOff;
            userToSet.RemainingSickDaysOff = initialDaysOffInfo.InitialSickDaysOff;
            userToSet.IsInitialDaysOffSet = true;

            await UpdateAsync(userToSet);
        }

        public async Task DecreaseRemainingDaysOff(string userId, int daysToRemove, RequestType type)
        {
            User userToUpdate = await Users.FirstOrDefaultAsync(u => u.Id == userId);

            switch (type)
            {
                case RequestType.Paid:
                    userToUpdate.RemainingPaidDaysOff -= daysToRemove;
                    break;
                case RequestType.Unpaid:
                    userToUpdate.RemainingUnpaidDaysOff -= daysToRemove;
                    break;
                case RequestType.Sick:
                    userToUpdate.RemainingSickDaysOff -= daysToRemove;
                    break;
                default:
                    break;
            }

            await UpdateAsync(userToUpdate);
        }

        public async Task IncreaseRemainingDaysOff(string userId, int daysToAdd, RequestType type)
        {
            User userToUpdate = await Users.FirstOrDefaultAsync(u => u.Id == userId);

            switch (type)
            {
                case RequestType.Paid:
                    userToUpdate.RemainingPaidDaysOff += daysToAdd;
                    break;
                case RequestType.Unpaid:
                    userToUpdate.RemainingUnpaidDaysOff += daysToAdd;
                    break;
                case RequestType.Sick:
                    userToUpdate.RemainingSickDaysOff += daysToAdd;
                    break;
                default:
                    break;
            }

            await UpdateAsync(userToUpdate);
        }
    }
}
