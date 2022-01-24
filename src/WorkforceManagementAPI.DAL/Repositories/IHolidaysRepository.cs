using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkforceManagementAPI.DAL.Repositories
{
    public interface IHolidaysRepository
    {
        Task<bool> Exists(DateTime dateTime);
    }
}
