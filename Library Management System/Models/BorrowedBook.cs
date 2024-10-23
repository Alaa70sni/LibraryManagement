namespace Library_Management_System.Models;



public class BorrowedBook
{
    public int Id { get; set; }
    public int MemberId { get; set; } // Foreign key to Member
    public int BookId { get; set; } // Foreign key to Book
    public int UserId { get; set; } // Foreign key to Book
    public DateTime BorrowedDate { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedDate { get; set; } // Nullable
    public decimal Penalty { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Book Book { get; set; }
}
