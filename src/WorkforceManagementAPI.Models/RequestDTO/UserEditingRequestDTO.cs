using System.ComponentModel.DataAnnotations;

namespace WorkforceManagementAPI.Models.RequestDTO
{
    public class UserEditingRequestDTO
    {
        [Required]
        [MinLength(1)]
        [MaxLength(150)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
    }
}
