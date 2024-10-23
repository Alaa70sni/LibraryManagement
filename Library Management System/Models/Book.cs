namespace Library_Management_System.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int AvailableCopies { get; set; }
    public int TotalCopies { get; set; }
    
    // Add this navigation property
    public ICollection<BorrowedBook> BorrowedBooks { get; set; }
    public virtual ICollection<Checkout> Checkouts { get; set; }



}
