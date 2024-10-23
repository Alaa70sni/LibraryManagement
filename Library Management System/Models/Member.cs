namespace Library_Management_System.Models;
using System;
using System.ComponentModel.DataAnnotations;

public class Member : User
{
    public int UserId { get; set; } // Foreign key to User
    public string Role { get; set; }
    public int BorrowedBooks { get; set; }
     public ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();

}
