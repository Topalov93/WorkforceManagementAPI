using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.DAL.Repositories
{
    public interface ITimeOffRequestRepository
    {
        Task<List<TimeOffRequest>> Get(Expression<Func<TimeOffRequest, bool>> action);
        Task<TimeOffRequest> GetSingle(Expression<Func<TimeOffRequest, bool>> action);
        Task<List<TimeOffRequest>> All();
        Task Create(TimeOffRequest timeOffRequest);
        Task Delete(int timeOffRequestId);
        Task Update(int timeOffRequestId, TimeOffRequest request);
        Task SetStatus(TimeOffRequest timeOffRequest, RequestStatus status);
    }
}
