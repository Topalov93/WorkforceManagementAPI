using System.ComponentModel.DataAnnotations;

namespace WorkforceManagementAPI.Models.RequestDTO
{
    public class InitialDaysOffDTO
    {

        [Range(0, 20, ErrorMessage = "Paid days off must be between 0 and 20!")]
        public int InitialPaidDaysOff { get; set; }

        [Range(0, 90, ErrorMessage = "Unpaid days off must be between 0 and 90!")]
        public int InitialUnpaidDaysOff { get; set; }

        [Range(0, 40, ErrorMessage = "Sick days off must be between 0 and 40!")]
        public int InitialSickDaysOff { get; set; }
    }
}
