using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.Common.ResultState;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Services
{
    public interface ITimeOffService
    {
        Task<List<TimeOffRequest>> GetAll();
        Task<List<TimeOffRequest>> Get(Expression<Func<TimeOffRequest, bool>> action);
        Task<TimeOffRequest> GetSingle(Expression<Func<TimeOffRequest, bool>> action);
        Task<ResultState> Create(TimeOffRequest timeOffRequest, string currentUserId);
        Task<ResultState> Delete(int timeOffRequestId);
        Task<ResultState> Update(int timeOffRequestId, TimeOffRequest request);
        Task<ResultState> SetApprovalStatus(string currentUserId, int timeOffRequestId, bool isApproved);
        Task<ResultState> SendApprovals(int timeOffRequestId, string currentUserId);
        Task<ResultState> CancelUserPendindRequests(User user);
        Task ScheduleRequestsDeletion(string userId);
    }
}
