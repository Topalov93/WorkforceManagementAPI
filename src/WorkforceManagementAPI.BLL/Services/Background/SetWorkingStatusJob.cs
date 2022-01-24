using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.DAL;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Services.Background
{
    [ExcludeFromCodeCoverage]
    public class SetWorkingStatusJob : IJob
    {
        private readonly WorkforceManagementAPIDbContext _context;

        public SetWorkingStatusJob(WorkforceManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                user.IsWorking = await IsUserWorking(user);
            }
            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsUserWorking(User user)
        {
            DateTime today = DateTime.UtcNow.Date;
            var approvedRequests = await _context.TimeOffRequests.Where(r => r.Creator.Id.Equals(user.Id) && r.Status == RequestStatus.Approved).ToListAsync();
            foreach (var request in approvedRequests)
            {
                if (request.StartDate.Date < today && request.EndDate.Date > today) return false;
            }

            return true;
        }
    }
}
