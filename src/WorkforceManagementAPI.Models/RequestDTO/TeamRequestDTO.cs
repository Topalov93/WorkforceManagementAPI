using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkforceManagementAPI.Models.RequestDTO
{
    public class TeamRequestDTO
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public Guid TeamLeaderId { get; set; }
    }
}
