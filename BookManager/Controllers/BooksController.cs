namespace BookManager.Controllers
{
    using BookAPI.Models;
    using BookManager.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Diagnostics;
    using System.Text;

    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;

        // Todo: Load this value from config.
        private const string _baseUrl = "https://localhost:7283";
        // Todo: Load this value from config.
        private const string _api = "bookapi";

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> List()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{_baseUrl}/{_api}/list");

                if (response.IsSuccessStatusCode)
                {
                    var data =  await response.Content.ReadAsStringAsync();

                    var books = JsonConvert.DeserializeObject<List<Book>>(data) ?? [];

                    return View("List", new BookViewModel(books));
                }
                else
                {
                    _logger.Log(LogLevel.Error, "Error listing books.");
                    return Error();
                }
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= default(int))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{_baseUrl}/{_api}/details/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    var book = JsonConvert.DeserializeObject<Book>(data);

                    return View(book);
                }
                else
                {
                    _logger.Log(LogLevel.Error, $"Error finding book {id}.");
                    return Error();
                }
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Save(string name, string author, int year)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException($"'{nameof(author)}' cannot be null or whitespace.", nameof(author));
            }

            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(new Book(default(int), name, author, year)), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{_baseUrl}/{_api}/create", content);

                if (response.IsSuccessStatusCode)
                {
                    return await List();
                }
                else
                {
                    _logger.Log(LogLevel.Error, "Error creating book.");
                    return Error();
                }
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= default(int))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            using (var client = new HttpClient())
            {
                var response = await client.DeleteAsync($"{_baseUrl}/{_api}/delete/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await List();
                }
                else
                {
                    _logger.Log(LogLevel.Error, $"Error deleting book {id}.");
                    return Error();
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}