using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkforceManagementAPI.Models.ResponseDTO
{
    public class TeamResponseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public string TeamLeader { get; set; }

        public List<string> Members { get; set; }

        public DateTime AddedOn { get; set; }

        public DateTime? EditedOn { get; set; }
    }
}
