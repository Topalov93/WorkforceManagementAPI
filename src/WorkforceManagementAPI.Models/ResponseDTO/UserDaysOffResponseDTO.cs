namespace WorkforceManagementAPI.Models.ResponseDTO
{
    public class UserDaysOffResponseDTO
    {
        public string User { get; set; }

        public int InitialPaidDaysOff { get; set; }

        public int InitialUnpaidDaysOff { get; set; }

        public int InitialSickDaysOff { get; set; }

        public int RemainingPaidDaysOff { get; set; }

        public int RemainingUnpaidDaysOff { get; set; }

        public int RemainingSickDaysOff { get; set; }
    }
}
