
namespace Library_Management_System.Models;
using System;
using System.ComponentModel.DataAnnotations;

public class Librarian : User
{
    public int UserId { get; set; } // This is the primary key
    public string Role { get; set; } // or any other relevant properties
}

