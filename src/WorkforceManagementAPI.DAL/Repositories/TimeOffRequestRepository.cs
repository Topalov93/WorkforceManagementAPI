using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.DAL.Repositories
{
    public class TimeOffRequestRepository : ITimeOffRequestRepository
    {
        private WorkforceManagementAPIDbContext _context;

        public TimeOffRequestRepository(WorkforceManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeOffRequest>> All()
        {
            return await _context.TimeOffRequests.ToListAsync();
        }

        public async Task Create(TimeOffRequest timeOffRequest)
        {
            await _context.TimeOffRequests.AddAsync(timeOffRequest);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int timeOffRequestId)
        {
            var result = await _context.TimeOffRequests.FindAsync(timeOffRequestId);
            _context.TimeOffRequests.Remove(result);

            await _context.SaveChangesAsync();
        }

        public async Task<List<TimeOffRequest>> Get(Expression<Func<TimeOffRequest,bool>> action)
        {
            return await _context.TimeOffRequests.Where(action).ToListAsync();
        }

        public async Task<TimeOffRequest> GetSingle(Expression<Func<TimeOffRequest, bool>> action)
        {
            return await _context.TimeOffRequests.SingleOrDefaultAsync(action);
        }

        public async Task SetStatus(TimeOffRequest timeOffRequest, RequestStatus status)
        {
            timeOffRequest.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task Update(int timeOffRequestId, TimeOffRequest request)
        {
            var timeOffRequest = await _context.TimeOffRequests.FindAsync(timeOffRequestId);
            timeOffRequest.ModifiedOn = DateTime.UtcNow;
            timeOffRequest.Type = request.Type;
            timeOffRequest.Description = request.Description;
            timeOffRequest.Duration = request.Duration;
            timeOffRequest.StartDate = request.StartDate;
            timeOffRequest.EndDate = request.EndDate;

            await _context.SaveChangesAsync();
        }
    }
}
