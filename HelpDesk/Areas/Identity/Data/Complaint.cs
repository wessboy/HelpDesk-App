namespace HelpDesk.Areas.Identity.Data
{
    public class Complaint
    {

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public DateTime DateCreated { get; set; }
        public DateTime? DateResolved { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
