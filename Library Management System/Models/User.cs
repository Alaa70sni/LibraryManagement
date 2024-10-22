namespace Library_Management_System.Models;
using System;
using System.ComponentModel.DataAnnotations;
   // public ICollection<BorrowedBook> BorrowedBooks { get; set; } // Reference to borrowed books

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Role { get; set; } // "Member" or "Librarian"
    public string Address { get; set; }
}
