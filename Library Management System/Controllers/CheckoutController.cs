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
    




        [HttpGet]
    public IActionResult MemberCheckout()
    {
        var memberId = GetCurrentMemberId(); // Method to get the current member ID
        var checkouts = _context.Checkouts
            .Where(c => c.MemberId == memberId)
            .Include(c => c.Book) // Include book details if needed
            .ToList();

        var viewModel = new CheckoutViewModel
        {
            Checkouts = checkouts,
            DueDate = DateTime.Now.AddDays(14) // Default due date (14 days later)
        };

        return View(viewModel);
    }

    private int GetCurrentMemberId()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
        return user?.Id ?? 0;
    }



[HttpPost]
public IActionResult SubmitCheckout(DateTime dueDate)
{
    var memberId = GetCurrentMemberId();
    var checkouts = _context.Checkouts
        .Where(c => c.MemberId == memberId)
        .ToList();

       foreach (var checkout in checkouts)
    {
        var borrowedBook = new BorrowedBook
        {
            BookId = checkout.BookId,
            MemberId = memberId,
            DueDate = dueDate,
            BorrowedDate = DateTime.Now
        };

        _context.BorrowedBooks.Add(borrowedBook);
    }

    _context.SaveChanges();

        // Optionally clear the checkouts after borrowing
    _context.Checkouts.RemoveRange(checkouts);
    _context.SaveChanges();

        // Redirect to the same page with a success message
    ViewBag.SuccessMessage = "The Books Borrowed Successfully";

    return View("MemberCheckout", new CheckoutViewModel { Checkouts = new List<Checkout>(), DueDate = dueDate });
}





    
[HttpPost]
public IActionResult Add(Checkout checkout)
{
    if (checkout == null || checkout.BookId == 0 || checkout.MemberId == 0)
    {
        ModelState.AddModelError("", "Invalid data.");
        return RedirectToAction("BookPage", "Books");
    }

    var book = _context.Books.Find(checkout.BookId);
    if (book == null || book.AvailableCopies <= 0)
    {
        ModelState.AddModelError("", "No available copies.");
        return RedirectToAction("BookPage", "Books");
    }

    // Create checkout entry
    checkout.CheckoutDate = DateTime.Now;
    checkout.DueDate = DateTime.Now.AddDays(14); // Set due date (14 days later)
    _context.Checkouts.Add(checkout);

    // Update book's available copies
    book.AvailableCopies--;
    _context.SaveChanges();

    return RedirectToAction("BookPage", "Books"); // Redirect back to book page
}


}
