using System;

namespace WorkforceManagementAPI.Models.ResponseDTO
{
    public class UserResponseDTO
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public DateTime AddedOn { get; set; }

        public DateTime? EditedOn { get; set; }
    }
}
