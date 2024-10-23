using System;
using System.ComponentModel.DataAnnotations;


namespace Library_Management_System.ViewModels
{
    public class RegisterViewModel
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public string Role { get; set; } // "Member" or "Librarian"
    public string Address { get; set; }
}




/*public class RegisterViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = " Not Match.")]
    public string ConfirmPassword { get; set;}

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    public string Gender { get; set; }

    [Required]
    public string Phone { get; set; }

    public string Address { get; set; }

    public string Role { get; set; } // Optional, e.g., "Member" or "Librarian"
}*/
}