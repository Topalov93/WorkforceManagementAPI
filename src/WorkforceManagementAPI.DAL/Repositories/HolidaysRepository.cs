using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.DAL.Repositories
{
    public class HolidaysRepository : IHolidaysRepository
    {
        private readonly WorkforceManagementAPIDbContext _context;
        public HolidaysRepository(WorkforceManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Exists(DateTime dateTime)
        {
            var exists = await _context.Holidays.SingleOrDefaultAsync(h => h.Holiday.Date == dateTime.Date);
            return exists != null;
        }
    }
}
