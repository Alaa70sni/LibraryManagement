using Microsoft.AspNetCore.Mvc;
using Library_Management_System.Data;
using Library_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Library_Management_System.Controllers
{
    public class LibrarianController : Controller
    {
        private readonly LibraryContext _context;

        public LibrarianController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Librarian/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Librarian/CheckedOutBooks
        public async Task<IActionResult> CheckedOutBooks()
        {
            var checkedOutBooks = await _context.Checkouts
                .Include(c => c.Book)
                .Include(c => c.Member)
                .ToListAsync();

            return View(checkedOutBooks);
        }

        // Other librarian-specific actions can be added here
    }
}
