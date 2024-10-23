using Library_Management_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Ensure this is included


namespace Library_Management_System.ViewModels
{
public class ProfileViewModel
{
    public User User { get; set; }
    public List<BorrowedBook> BorrowedBooks { get; set; }
}
}