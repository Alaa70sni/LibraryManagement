namespace PresentationLayer.Models
{
    public class Checkout
    {
        public int CheckoutId { get; set; } // Primary Key
        public DateTime CheckoutDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }

        // Foreign Keys
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; }
    }
}
