using WorkforceManagementAPI.Common.Enums;

namespace WorkforceManagementAPI.DAL.Entities
{
    public class Approval
    {
        public int Id { get; set; }

        public ApprovalStatus Status { get; set; }

        public string ApproverId { get; set; }

        public virtual TimeOffRequest TimeOffRequest { get; set; }
    }
}
