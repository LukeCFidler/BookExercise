using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Models
{
    public class Book
    {
        public int Id { set; get; }

        [FromBody]
        public string Name { set; get; }

        [FromBody]
        public string Author { set; get; }

        [FromBody]
        public int Year { set; get; }

        public Book() { }

        public Book(int id, string name, string author, int year)
        {
            Id = id;
            Name = name;
            Author = author;
            Year = year;
        }
    }
}
