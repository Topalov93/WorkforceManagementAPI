using System;
using System.Collections.Generic;

namespace WorkforceManagementAPI.DAL.Entities
{
    public class Team
    {
        public Team()
        {
            TeamMembers = new List<User>();
        }

        public int Id { get; set; }

        public virtual User TeamLeader { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public virtual List<User> TeamMembers { get; set; }
    }
}
