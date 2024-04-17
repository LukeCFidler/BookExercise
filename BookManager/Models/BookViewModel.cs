namespace BookManager.Models
{
    using BookAPI.Models;

    public class BookViewModel
    {
        public List<Book> Books { get; private set; } 

        public BookViewModel(List<Book> books)
        {
            Books = books ?? throw new ArgumentNullException(nameof(books));  
        }
    }
}
