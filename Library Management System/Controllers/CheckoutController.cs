using Microsoft.AspNetCore.Mvc;
using Library_Management_System.Data;
using Library_Management_System.Models;
using Library_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore; 
using System;

public class CheckoutController : Controller
{
    private readonly LibraryContext _context;

    public CheckoutController(LibraryContext context)
    {
        _context = context;

    }
    

        private string GetCurrentUserId()
    {
        return HttpContext.Session.GetString("UserId");
    }

    private string GetCurrentUserRole()
    {
        return HttpContext.Session.GetString("UserRole");
    }


    private int GetCurrentMemberId()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
        return user?.Id ?? 0;
    }



  public async Task<IActionResult> LibrarianCheckout()
{
    var userId = GetCurrentUserId();
    var userRole = GetCurrentUserRole();

    var checkouts = await _context.Checkouts
                .Where(c => c.DueDate == null) // Only include checkouts with null due dates
                .Include(c => c.Member)
                .Include(c => c.Book)
                .ToListAsync();

    var viewModel = new CheckoutViewModel
    {
        Checkouts = checkouts ?? new List<Checkout>(), // Ensure Checkouts is initialized
    };

    return View(viewModel);
}


[HttpGet]
public async Task<IActionResult> MemberCheckout()
{
    var memberId = GetCurrentMemberId(); // Get current member ID
    var checkouts = await _context.Checkouts
        .Where(c => c.MemberId == memberId && c.DueDate == null)
        .Include(c => c.Book)
        .ToListAsync();

    var viewModel = new CheckoutViewModel
    {
        Checkouts = checkouts ?? new List<Checkout>(), // Ensure Checkouts is initialized
    };

    return View(viewModel);
}


public IActionResult MemberDetails(int id)
{
    // Fetch the member from the database using the provided ID
    var member = _context.Members.Find(id);

    if (member == null)
    {
        return NotFound(); // Return 404 if the member is not found
    }

    return View(member); // Pass the member object to the view
}




    
[HttpPost]
public async Task<IActionResult> SetDueDate(DateTime dueDate)
{

    // Validate the due date range
    if (dueDate < new DateTime(1753, 1, 1) || dueDate > new DateTime(9999, 12, 31))
    {
        ModelState.AddModelError("DueDate", "Due date is out of the acceptable range.");
        return View("MemberCheckout"); // Return to the view with the error
    }

    var memberId = GetCurrentMemberId();
    var checkouts = await _context.Checkouts
        .Where(c => c.MemberId == memberId && c.DueDate == null)
        .ToListAsync();

    if (!checkouts.Any())
    {
        ModelState.AddModelError("", "No checkouts found for the current member.");
        return View("MemberCheckout"); // Return with an error
    }

    else
    {
        foreach (var checkout in checkouts)
        {
            checkout.DueDate = dueDate;

            var borrowedBook = new BorrowedBook
            {
                BookId = checkout.BookId,
                MemberId = memberId,
                DueDate = dueDate,
                CheckoutDate = checkout.CheckoutDate,
                BorrowedDate = DateTime.Now
            };
            _context.BorrowedBooks.Add(borrowedBook);
        }

        await _context.SaveChangesAsync();
    }

    return RedirectToAction("MemberCheckout");
}



    
[HttpPost]
public IActionResult Add(Checkout checkout)
{
    if (checkout == null || checkout.BookId == 0)
    {
        ModelState.AddModelError("", "Invalid data.");
        return RedirectToAction("BookPage", "Books");
    }

    checkout.MemberId = GetCurrentMemberId(); // Get the current member ID

    var book = _context.Books.Find(checkout.BookId);
    if (book == null || book.AvailableCopies <= 0)
    {
        ModelState.AddModelError("", "No available copies.");
        return RedirectToAction("BookPage", "Books");
    }

    // Set checkout date to now
    checkout.CheckoutDate = DateTime.Now;

    // Create checkout entry
    _context.Checkouts.Add(checkout);

    // Update book's available copies
    book.AvailableCopies--;
    _context.SaveChanges();

    return RedirectToAction("BookPage", "Books");
}

[HttpPost]
public async Task<IActionResult> RemoveCheckout(int checkoutId)
{
    var checkout = await _context.Checkouts.FindAsync(checkoutId);
    if (checkout != null)
    {
        _context.Checkouts.Remove(checkout);

        // Assuming you have a Book entity and DbSet<Book>
        var book = await _context.Books.FindAsync(checkout.BookId);
        if (book != null)
        {
            book.AvailableCopies += 1; // Increment available copies
        }

        await _context.SaveChangesAsync();
    }

    return RedirectToAction("MemberCheckout"); // Or whichever action to refresh the list
}



}
