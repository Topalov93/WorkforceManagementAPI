using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();

        Task<List<User>> GetDeletedUsers();

        Task<User> GetUserById(Guid userId);

        Task<ResultState> CreateUser(User userToAdd, string password, string role);

        Task<ResultState> EditUser(Guid userId, User userToEdit, Guid currentUserId);

        Task<ResultState> DeleteUser(Guid userId, Guid currentUserId);

        Task<ResultState> SetInitialDaysOff(Guid userId, User initialDaysOffInfo);

        Task<List<string>> GetUserRole(User user);
    }
}
