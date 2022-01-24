using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagementAPI.BLL.Managers;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserManager _userManager;
        private readonly ITeamService _teamService;
        private readonly ITimeOffService _timeOffService;

        public UserService(IUserManager userManager, ITeamService teamService, ITimeOffService timeOffService)
        {
            _userManager = userManager;
            _teamService = teamService;
            _timeOffService = timeOffService;
        }
        public async Task<ResultState> CreateUser(User newUser, string password, string role)
        {
            if (await _userManager.FindByNameAsync(newUser.UserName) != null)
            {
                return new ResultState(false, "User already exist!");
            }

            var roles = await _userManager.GetAllRoles();

            if (!roles.Any(r => r.NormalizedName == role.ToUpper()))
            {
                return new ResultState(false, "Role not supported!");
            }

            newUser.IsWorking = true;
            newUser.EmailConfirmed = true;

            try
            {
                await _userManager.CreateUserAsync(newUser, password, role);
                return new ResultState(true, "User was successfully created!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to create user!", ex);
            }
        }

        public async Task<ResultState> EditUser(Guid userId, User newInfo, Guid currentUserId)
        {
            User userToEdit = await _userManager.FindByIdAsync(userId.ToString());

            if (userToEdit is null || userToEdit.IsDeleted)
            {
                return new ResultState(false, "User does not exist!");
            }

            if (userToEdit.Email == "admin@test.test")
            {
                return new ResultState(false, "Can not edit initial admin user!");
            }

            if (await _userManager.FindByNameAsync(newInfo.UserName) != null)
            {
                return new ResultState(false, "Email already exist!");
            }

            newInfo.ModifiedOn = DateTime.UtcNow;

            try
            {
                await _userManager.EditUserById(userId.ToString(), newInfo);
                return new ResultState(true, "User was successfully edited!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to edit user!", ex);
            }
        }

        public async Task<ResultState> DeleteUser(Guid userToDeleteId, Guid currentUserId)
        {
            User userToDelete = await _userManager.FindByIdAsync(userToDeleteId.ToString());

            if (userToDelete is null || userToDelete.IsDeleted)
            {
                return new ResultState(false, "User does not exist!");
            }

            if (userToDelete.Email == "admin@test.test")
            {
                return new ResultState(false, "Can not delete initial admin user!");
            }

            var userTeams = userToDelete.Teams.ToList();

            if (userTeams.Count > 0)
            {
                foreach (var team in userTeams)
                {
                    try
                    {
                        await _teamService.RemoveMember(team.Id, userToDeleteId);
                    }
                    catch (Exception ex)
                    {
                        return new ResultState(false, "Can not delete user, because she is member of a team!", ex);
                    }
                }
            }

            if (userToDelete.LeadTeams.Count > 0)
            {
                return new ResultState(false, "Can not delete user, because she is leader of a team!");
            }

            var userRequests = userToDelete.CreatedRequests.ToList();

            if (userRequests.Count > 0)
            {
                try
                {
                    await _timeOffService.CancelUserPendindRequests(userToDelete);
                    await _timeOffService.ScheduleRequestsDeletion(userToDeleteId.ToString());
                }
                catch (Exception ex)
                {
                    return new ResultState(false, "Can not delete user, because she has created time off requests!", ex);
                }
            }

            var userRequestForApproval = userToDelete.RequestsForApproval.ToList();

            if (userRequestForApproval.Count > 0)
            {
                return new ResultState(false, "Can not delete user, because she has time off requests for approval!");
            }

            try
            {
                await _userManager.DeleteUserBy(userToDeleteId.ToString());
                return new ResultState(true, "User was successfully deleted!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to delete user!", ex);
            }
        }

        public async Task<ResultState> SetInitialDaysOff(Guid userId, User initialDaysOffInfo)
        {
            User userToEdit = await _userManager.FindByIdAsync(userId.ToString());

            if (userToEdit is null || userToEdit.IsDeleted)
            {
                return new ResultState(false, "User does not exist!");
            }

            if (userToEdit.IsInitialDaysOffSet)
            {
                return new ResultState(false, "Initial days off already updated!");
            }

            initialDaysOffInfo.ModifiedOn = DateTime.UtcNow;

            try
            {
                await _userManager.SetInitialDaysOff(userId.ToString(), initialDaysOffInfo);
                return new ResultState(true, "Initial days off successfully set!");
            }
            catch (Exception ex)
            {
                return new ResultState(false, "Unable to set initial days off!", ex);
            }
        }

        public async Task<List<User>> GetUsers()
        {
            var users =  await _userManager.GetAllAsync();
            var userToShow = users.Where(u => u.IsDeleted == false).ToList();
            return userToShow;
        }

        public async Task<List<User>> GetDeletedUsers()
        {
            var users = await _userManager.GetAllAsync();
            var userToShow = users.Where(u => u.IsDeleted == true).ToList();
            return userToShow;
        }

        public async Task<User> GetUserById(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<List<string>> GetUserRole(User user)
        {
            return await _userManager.GetUserRolesAsync(user);
        }
    }
}
