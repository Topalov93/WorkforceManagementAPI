using System.ComponentModel.DataAnnotations;

namespace WorkforceManagementAPI.Models.RequestDTO
{
    public class UserCreationRequestDTO : UserEditingRequestDTO
    {
        [Required]
        [MinLength(1)]
        [MaxLength(32)]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Role { get; set; }
    }
}
    