using Library_Management_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Ensure this is included


namespace Library_Management_System.ViewModels
{

public class BookPageViewModel
{
    public IEnumerable<Book> Books { get; set; }
    public string UserRole { get; set; }
    public int MemberId { get; set; }
}
}