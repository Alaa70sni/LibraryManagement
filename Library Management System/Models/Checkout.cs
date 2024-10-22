namespace Library_Management_System.Models;


public class Checkout
{
    public int Id { get; set; }
    public int BookId { get; set; } // Foreign key to Book
    public int MemberId { get; set; } // Foreign key to Member
    public DateTime CheckoutDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedDate { get; set; } // Nullable
    public bool ReturnedState { get; set; }
    public decimal PenaltyExpenses { get; set; }
    public Book Book { get; set; }
    public Member Member { get; set; } // Navigation Property

        public decimal CalculatePenalty(decimal penaltyRate)
    {
        if (!ReturnedDate.HasValue || ReturnedDate <= DueDate)
        {
            return 0; // No penalty if not overdue or not returned
        }

        int overdueDays = (ReturnedDate.Value - DueDate).Days;
        return overdueDays * penaltyRate;
    }
    
}