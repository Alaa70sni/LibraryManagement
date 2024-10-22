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

    public IActionResult BookPage()
    {
        var books = _context.Books.ToList();
        var memberId = GetCurrentMemberId(); // New method to get the member ID


        var viewModel = new BookPageViewModel
    {
        Books = books,
        MemberId = memberId // Add MemberId to the view model
    };

        return View(books);
    }

    private int GetCurrentMemberId()
{
     var userEmail = HttpContext.Session.GetString("UserEmail");
    var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
    return user?.Id ?? 0; 
}



    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(BookPage));
        }
        return View(book);
    }

        // GET: Books/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

        // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: Books/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
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
