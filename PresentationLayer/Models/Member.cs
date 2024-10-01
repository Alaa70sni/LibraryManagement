namespace PresentationLayer.Models
{
    public class Member
    {
        public int MemberId { get; set; } // Primary Key
        public string Name { get; set; }
        public string Email { get; set; }

        // Navigation Property
        public ICollection<Checkout> Checkouts { get; set; }
    }
}
