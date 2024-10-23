namespace Library_Management_System.ViewModels
{
    public class EditProfileViewModel
    {
    public int Id { get; set; } // Add this line
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    }
}
