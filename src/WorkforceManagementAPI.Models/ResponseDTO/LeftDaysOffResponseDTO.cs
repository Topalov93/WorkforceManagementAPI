using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkforceManagementAPI.Models.ResponseDTO
{
    public class LeftDaysOffResponseDTO
    {
        public int PaidDaysOff { get; set; }

        public int UnpaidDaysOff { get; set; }

        public int SickDaysOff { get; set; }
    }
}
