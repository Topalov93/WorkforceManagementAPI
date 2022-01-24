using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Services
{
    public interface IEmailService
    {
        Task<ResultState> SendEmail(string receiver, string subject, string template);

        Task<ResultState> SendEmailRequestToTeamLeaders(List<User> teamLeaders, TimeOffRequest timeOffRequest);

        Task<ResultState> SendEmailNotificationToTeamLeaders(List<User> teamLeaders, TimeOffRequest timeOffRequest, bool isApproved);

        Task<ResultState> SendEmailNotificationToTeamsMembers(List<Team> teams, string currentUserId, TimeOffRequest timeOffRequest, bool isApproved);

        Task<ResultState> SendEmailNotificationToUser(User user, TimeOffRequest timeOffRequest, bool isApproved);
    }
}
