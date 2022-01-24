using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WorkforceManagementAPI.DAL.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            Teams = new List<Team>();
            LeadTeams = new List<Team>();
            CreatedRequests = new List<TimeOffRequest>();
            RequestsForApproval = new List<TimeOffRequest>();
        }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public bool IsWorking { get; set; }

        public int InitialPaidDaysOff { get; set; }

        public int InitialUnpaidDaysOff { get; set; }

        public int InitialSickDaysOff { get; set; }

        public int RemainingPaidDaysOff { get; set; }

        public int RemainingUnpaidDaysOff { get; set; }

        public int RemainingSickDaysOff { get; set; }

        public bool IsInitialDaysOffSet { get; set; }

        public bool IsDeleted { get; set; }

        public virtual List<Team> Teams { get; set; }

        public virtual List<Team> LeadTeams { get; set; }

        public virtual List<TimeOffRequest> CreatedRequests { get; set; }

        public virtual List<TimeOffRequest> RequestsForApproval{ get; set; }
    }
}
