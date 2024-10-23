using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Library_Management_System.Models;
using Library_Management_System.Data;
using Microsoft.EntityFrameworkCore; // Required for EF Core async methods
using Library_Management_System.ViewModels;


namespace Library_Management_System.Controllers;


public class BooksController : Controller
{
    private readonly LibraryContext _context;

    public BooksController(LibraryContext context)
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

    public IActionResult BookPage()
    {
        var books = _context.Books.ToList();
        var memberId = GetCurrentMemberId();
        var userRole = GetCurrentUserRole(); // Get the current user role
            ViewBag.UserRole = userRole; // Set UserRole in ViewBag // New method to get the member ID


        var viewModel = new BookPageViewModel
    {
        Books = books,
        MemberId = memberId // Add MemberId to the view model
    };

        return View(books);
    }

[HttpGet]
public IActionResult Create()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Book newBook)
{
    if (newBook == null)
    {
        ModelState.AddModelError("", "Invalid data.");
        return View(newBook);
    }

    if (ModelState.IsValid)
    {
        // Check if the book already exists (optional)
        var existingBook = _context.Books
            .FirstOrDefault(b => b.Title == newBook.Title && b.Author == newBook.Author);
        if (existingBook != null)
        {
            ModelState.AddModelError("", "This book already exists.");
            return View(newBook);
        }

        // Add new book to the context
        _context.Books.Add(newBook);
        _context.SaveChanges(); // Save changes synchronously

        return RedirectToAction("BookPage"); // Redirect to the list of books after creating
    }

    // Return the view with the model if validation fails
    return View(newBook);
}

// GET: Books/Edit
[HttpGet]
public IActionResult Edit(int id)
{
    var book = _context.Books.Find(id);
    if (book == null)
    {
        return NotFound();
    }

    var editBookViewModel = new EditBookViewModel
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Genre = book.Genre,
        AvailableCopies = book.AvailableCopies,
        TotalCopies = book.TotalCopies
    };

    return View("EditBook", editBookViewModel); // Use the correct view name
}


    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(EditBookViewModel model)
{
    if (ModelState.IsValid)
    {
        var book = await _context.Books.FindAsync(model.Id);
        if (book == null)
        {
            return NotFound();
        }

        book.Title = model.Title;
        book.Author = model.Author;
        book.Genre = model.Genre;
        book.AvailableCopies = model.AvailableCopies;
        book.TotalCopies = model.TotalCopies;

        await _context.SaveChangesAsync();

        return RedirectToAction("BookPage"); // Redirect after successful edit
    }

    return View("EditBook", model); // Return the view with the model if validation fails
}

public async Task<IActionResult> Delete(int id)
{
    var book = await _context.Books
        .Include(b => b.Checkouts)
        .Include(b => b.BorrowedBooks)
        .FirstOrDefaultAsync(b => b.Id == id);

    if (book == null)
    {
        return NotFound();
    }

    return PartialView("_DeleteModal", book); // Ensure this partial view exists
}


public async Task<IActionResult> DeleteConfirmed(int id)
{
    var book = await _context.Books
        .Include(b => b.Checkouts) // Include related Checkouts
        .Include(b => b.BorrowedBooks) // Include related BorrowedBooks
        .FirstOrDefaultAsync(b => b.Id == id);
        
    if (book == null)
    {
        return NotFound();
    }

    // Check for related records
    if (book.Checkouts.Any() || book.BorrowedBooks.Any())
    {
        TempData["ErrorMessage"] = "Cannot delete this book as it is currently checked out or borrowed.";
        return RedirectToAction(nameof(BookPage)); // Redirect back to the book page or index
    }

    // If no related records, proceed to delete
    _context.Books.Remove(book);
    await _context.SaveChangesAsync();
    TempData["SuccessMessage"] = "Book deleted successfully!";

    return RedirectToAction(nameof(BookPage)); // Redirect back to the book page or index
}




    
private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }

public async Task<IActionResult> Search(string searchString)
{
    // Start with all books
    var booksQuery = from b in _context.Books
                     select b;

    // Filter books based on search string
    if (!string.IsNullOrEmpty(searchString))
    {
        booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(searchString.ToLower()) || 
                                            b.Author.ToLower().Contains(searchString.ToLower()) || 
                                            b.Genre.ToLower().Contains(searchString.ToLower()));
    }

    // Get distinct genres and authors from the database
    ViewBag.Genres = await _context.Books.Select(b => b.Genre).Distinct().ToListAsync();
    ViewBag.Authors = await _context.Books.Select(b => b.Author).Distinct().ToListAsync();

    // Execute the query and return the filtered list of books
    var books = await booksQuery.ToListAsync();
    return View(books);
}


public IActionResult About()
    {
        return View();
    }





}
