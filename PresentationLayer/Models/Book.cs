namespace PresentationLayer.Models
{
    public class Book
    {
        public int BookId { get; set; } // Primary Key
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }

        // Navigation Property
        public ICollection<Checkout> Checkouts { get; set; }
    }
}
