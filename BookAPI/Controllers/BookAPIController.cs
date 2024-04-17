namespace BookAPI.Controllers
{
    using BookAPI.DBContext;
    using BookAPI.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class BookAPIController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BookAPIController(BookDbContext context) 
        { 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("list")]
        public IEnumerable<Book> List()
        {
            return _context.List();
        }

        [HttpGet("details/{id}")]
        public Book Details(int id)
        {
            if (id <= default(int))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return _context.Details(id);
        }

        [HttpPost("create")]

        public int Create([FromBody]Book book)
        {
            if (book is null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            return _context.Create(book.Name, book.Author, book.Year);
        }

        [HttpDelete("delete/{id}")]
        public void Delete(int id)
        {
            if (id <= default(int))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            _context.Delete(id);
        }
    }
}
