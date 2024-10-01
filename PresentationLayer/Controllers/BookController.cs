using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Data; // Namespace for ApplicationDbContext
using PresentationLayer.Models; // Namespace for your Book model
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }
    }

}
