using System;
using System.ComponentModel.DataAnnotations;
using WorkforceManagementAPI.Common.Enums;
using WorkforceManagementAPI.Models.Attributes;

namespace WorkforceManagementAPI.Models.RequestDTO
{
    public class TimeOffRequestRequestDTO
    {
        [Required]
        [RequestTypeRange]
        public RequestType Type { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        [MinDateToday]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyy}")]
        public DateTime StartDate { get; set; }

        [Required]
        [MinDateToday]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyy}")]
        public DateTime EndDate { get; set; }

    }
}
