using System;
using System.Collections.Generic;
using WorkforceManagementAPI.Common.Enums;

namespace WorkforceManagementAPI.DAL.Entities
{
    public class TimeOffRequest
    {
        public TimeOffRequest()
        {
            Approvers = new List<User>();
            Approvals = new();
        }

        public int Id { get; set; }

        public virtual User Creator { get; set; }

        public RequestStatus Status { get; set; }

        public RequestType Type { get; set; }

        public string Description { get; set; }

        public virtual List<User> Approvers { get; set; }

        public virtual List<Approval> Approvals { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Duration { get; set; }

        public IEnumerable<string> LeadersEmails { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
