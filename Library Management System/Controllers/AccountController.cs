using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Library_Management_System.Models;
using Library_Management_System.ViewModels;
using Library_Management_System.Data;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly LibraryContext _context;

    public AccountController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Check if user already exists
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email is already taken.");
                return View(model);
            }

            // Create new user
            var user = new User
            {
                Name = model.Name,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                Email = model.Email,
                Password = HashPassword(model.Password), // Hash the password
                Phone = model.Phone,
                RegistrationDate = DateTime.Now,
                Role = model.Role, // Default role
                Address = model.Address
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login","Account");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user != null && VerifyPassword(password, user.Password))
        {
            // Store user information in session or cookie as needed
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetInt32("UserId", user.Id); // Store UserId if available

            return RedirectToAction("BookPage", "Books"); // Redirect to home page
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
    }

    private string HashPassword(string password)
    {
        if (password.Length !=8)
        {
            throw new ArgumentException( "password must be 8 characters.");

        }
        // Use a hashing algorithm (e.g., SHA256) to hash the password
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    private bool VerifyPassword(string inputPassword, string storedHash)
    {
        // Hash the input password and compare with stored hash
        return HashPassword(inputPassword) == storedHash;
    }

    public IActionResult Logout()
{
    HttpContext.Session.Clear(); // Clear the session
    return RedirectToAction("Login", "Account"); // Redirect to login page
}


     [HttpGet]
public IActionResult Profile()
{
    // Assuming you're storing user information in the session or cookie
    var email = HttpContext.Session.GetString("UserEmail"); // or however you're tracking the logged-in user
    var user = _context.Users.FirstOrDefault(u => u.Email == email);

    if (user == null)
    {
        return NotFound(); // User not found
    }

    List<BorrowedBook> borrowedBooks = new List<BorrowedBook>();
    if (user.Role == "Member")
    {
        // Retrieve borrowed books with book details
        borrowedBooks = _context.BorrowedBooks
            .Include(bb => bb.Book) // Include the related Book entity
            .Where(bb => bb.MemberId == user.Id) // Filter by user ID
            .ToList();
    }

    // Create a view model
    var profileViewModel = new ProfileViewModel
    {
        User = user,
        BorrowedBooks = borrowedBooks
    };
    
    ViewBag.MemberId = user.Id;

    return View(profileViewModel);
}

[HttpGet]
public IActionResult EditProfile()
{
    var email = HttpContext.Session.GetString("UserEmail");

        // Check if the email is null (user not logged in)
    if (string.IsNullOrEmpty(email))
    {
        return RedirectToAction("Login", "Account"); // Redirect to login page if not logged in
    }

    
    var user = _context.Users.FirstOrDefault(u => u.Email == email);

    if (user == null)
    {
        return NotFound(); // User not found
    }

    var model = new RegisterViewModel
    {
        Name = user.Name,
        BirthDate = user.BirthDate,
        Gender = user.Gender,
        Email = user.Email,
        Phone = user.Phone,
        Address = user.Address
    };


    return View(model);
}

[HttpPost]
public IActionResult EditProfile(RegisterViewModel model)
{
    if (!ModelState.IsValid)
    {
        // Log validation errors
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        foreach (var error in errors)
        {
            Console.WriteLine(error);
        }
        return View(model); // Return to view with errors
    }

    var email = HttpContext.Session.GetString("UserEmail");
    if (string.IsNullOrEmpty(email))
    {
        return RedirectToAction("Login", "Account"); // Redirect to login page
    }

    var user = _context.Users.FirstOrDefault(u => u.Email == email);
    if (user == null)
    {
        return NotFound(); // User not found
    }

    // Update user properties
    user.Name = model.Name;
    user.BirthDate = model.BirthDate;
    user.Gender = model.Gender;
    user.Phone = model.Phone;
    user.Address = model.Address;

    try
    {
        _context.SaveChanges(); // Save changes to the database
        return RedirectToAction("Profile", "Account"); // Redirect to profile page
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        ModelState.AddModelError(string.Empty, "Error saving changes. Please try again.");
        return View(model); // Return to view with error message
    }
}




[HttpPost]
public IActionResult ReturnBook(int borrowedBookId)
{
    var borrowedBook = _context.BorrowedBooks.FirstOrDefault(bb => bb.Id == borrowedBookId);
    if (borrowedBook != null && borrowedBook.ReturnedDate == null)
    {
        borrowedBook.ReturnedDate = DateTime.Now; // Set the returned date to now
        _context.SaveChanges(); // Save changes to the database
    }

    return RedirectToAction("Profile"); // Redirect back to the profile page
}


[HttpGet]
public IActionResult MemberCheckout()
{
    var email = HttpContext.Session.GetString("UserEmail");
    var user = _context.Users.FirstOrDefault(u => u.Email == email);
    
    if (user == null)
    {
        return RedirectToAction("Login", "Account"); // Redirect to login if user not found
    }

    var borrowedBooks = _context.BorrowedBooks
        .Where(bb => bb.MemberId == user.Id)
        .Include(bb => bb.Book)
        .ToList();

    return View(borrowedBooks);
}


[HttpPost]
public IActionResult SubmitCheckout([FromBody] List<BorrowedBook> borrowedBooks)
{
    if (borrowedBooks == null || !borrowedBooks.Any())
    {
        return Json(new { success = false, message = "No books selected." });
    }

    // Get the current user's email from the session
    var email = HttpContext.Session.GetString("UserEmail");
    var user = _context.Users.FirstOrDefault(u => u.Email == email);
    
    if (user == null)
    {
        return Json(new { success = false, message = "User not found." });
    }

    foreach (var book in borrowedBooks)
    {
        var newBorrowedBook = new BorrowedBook
        {
            BookId = book.BookId,
            MemberId = user.Id, // Get the member ID from the user object
            BorrowedDate = DateTime.Now,
            DueDate = book.DueDate
        };

        _context.BorrowedBooks.Add(newBorrowedBook);
    }

    _context.SaveChanges();
    return Json(new { success = true });
}






public IActionResult SomeAction()
{
    var email = HttpContext.Session.GetString("UserEmail");
    if (email == null)
    {
        // User is not logged in; redirect to login page
        return RedirectToAction("Login", "Account");
    }

    // User is logged in; proceed with the action
    // You can also retrieve user details from the database if needed
    var user = _context.Users.FirstOrDefault(u => u.Email == email);
    // Use user information as needed
    return View(user);
}









}

























/*using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Library_Management_System.Models; // Update with your actual namespace
using Library_Management_System.ViewModels; // Update with your actual namespace
using Library_Management_System.Data;
using Microsoft.EntityFrameworkCore; // Your database context

namespace Library_Management_System.Controllers
{
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly LibraryContext _context; // Your DbContext

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, LibraryContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username or password is wrong");
                return View(model);
            }
            var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (user == null && validPassword == false)
            {
                ModelState.AddModelError(string.Empty, "password is wrong");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password,model.RememberMe,false);
            if (result.Succeeded)
            {
                return RedirectToAction("BookPage","Books");
            }
            return View(model);
        }
        return View(model);
    }

    // Registration

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if(!ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                var newUser = new ApplicationUser()
                {
                    Name = model.Name,
                    BirthDate = model.BirthDate,
                    Gender = model.Gender,
                    PhoneNumber = model.Phone,
                    Email = model.Email,
                    UserName = model.Email,
                    Role = model.Role,
                    Address = model.Address,
                    RegistrationDate = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(newUser,model.Password);
                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser,isPersistent:false);
                    return RedirectToAction("BookPage" , "Books");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty,error.Description);
                }
                return View(model);

            }
            ModelState.AddModelError(String.Empty,"UserEmail Is Already Exist");
            return View(model);
        }

        return View(model);

    }
    [HttpPost]
    public async Task <IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("BookPage","Books");

    }
     

    
    }
}





/*
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                PhoneNumber = model.Phone,
                Address = model.Address,
                RegistrationDate = DateTime.UtcNow,
                Role = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Save additional user info to Users table
                var newUser = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    BirthDate = model.BirthDate,
                    Gender = model.Gender,
                    Address = model.Address,
                    Role = model.Role,
                    RegistrationDate = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Retrieve user info from Users table
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    // You can store user info in session or claims if needed
                    HttpContext.Session.SetString("UserName", user.Name);
                    HttpContext.Session.SetString("UserRole", user.Role);
                }

                return RedirectToAction("BookPage", "Books");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }
}
*/