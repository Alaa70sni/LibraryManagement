

namespace Library_Management_System.Models
{

public class CheckoutViewModel
{
    public int BookId { get; set; }
    public string MemberId { get; set; }
    public DateTime DueDate { get; set; }
    public List<Book> Books { get; set; } = new List<Book>(); // Ensure this property exists
    public List<Member> Members { get; set; } = new List<Member>(); 
    public List<Checkout> Checkouts { get; set; } // Assuming Checkout has Book and DueDate properties
    public bool IsSubmitted { get; set; } // Add this property


   
}

}
