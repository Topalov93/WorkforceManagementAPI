using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkforceManagementAPI.Models.ResponseDTO
{
    public class TimeOffRequestResponseDTO
    {
        public int Id { get; set; }

        public string CreatorId { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public List<ApprovalResponseDTO> Approvals { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Duration { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
