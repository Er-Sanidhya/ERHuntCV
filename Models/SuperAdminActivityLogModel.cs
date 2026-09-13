namespace ERHuntCV.Models
{
    public class SuperAdminActivityLogModel
    {
        public int ActivityLogID { get; set; }

        public int SuperAdminID { get; set; }

        public string? AccessType { get; set; }

        public string? IPAddress { get; set; }

        public string? Location { get; set; }

        public DateTime DateTime { get; set; }
    }
}
