

namespace Library_Management_System.Models
{

public class CheckoutViewModel
{
    public int BookId { get; set; }
    public string MemberId { get; set; }
    public DateTime DueDate { get; set; }
    public string Role { get; set; } // "Member" or "Librarian"
    public List<Book> Books { get; set; } = new List<Book>(); // Ensure this property exists
    public List<Member> Members { get; set; } = new List<Member>(); 
    public List<Checkout> Checkouts { get; set; } = new List<Checkout>(); // Ensure this is initialized
    public bool IsSubmitted { get; set; } // Add this property
 


   
}

}
