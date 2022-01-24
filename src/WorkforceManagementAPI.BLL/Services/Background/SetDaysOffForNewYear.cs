using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagementAPI.DAL;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.BLL.Services.Background
{
    [ExcludeFromCodeCoverage]
    public class SetDaysOffForNewYear : IJob
    {
        private readonly WorkforceManagementAPIDbContext _context;

        public SetDaysOffForNewYear(WorkforceManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                user.InitialPaidDaysOff = user.RemainingPaidDaysOff + 20;
                user.InitialUnpaidDaysOff = 90;
                user.InitialSickDaysOff = 40;
                user.RemainingPaidDaysOff = user.InitialPaidDaysOff;
                user.RemainingUnpaidDaysOff = user.InitialUnpaidDaysOff;
                user.RemainingSickDaysOff = user.InitialSickDaysOff;
            }

            await _context.SaveChangesAsync();
        }
    }
}
