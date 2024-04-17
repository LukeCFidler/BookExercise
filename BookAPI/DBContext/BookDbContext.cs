namespace BookAPI.DBContext
{
    using BookAPI.Models;
    using System;
    using System.Data.Entity;
    using System.Data.SQLite;

    public class BookDbContext : DbContext
    {
        private SQLiteConnection _connection;

        private int _latestId = 0;
        private const string _tableName = "book";
        private const string _databaseName = "main";

        public BookDbContext()
        {
            _connection = new SQLiteConnection("Data Source=:memory:");
            _connection.Open();

            ExecuteNonQuery(
                @$"CREATE TABLE {_databaseName}.{_tableName}(" +
                "id INT PRIMARY KEY NOT NULL," +
                "name TEXT NOT NULL," +
                "author TEXT NOT NULL," +
                "year INT NOT NULL);");
        }

        public int Create(string name, string author, int year)
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

            ExecuteNonQuery(@$"INSERT INTO {_databaseName}.{_tableName} (id, name, author, year) VALUES ({++_latestId}, '{name}','{author}',{year});");

            return _latestId;
        }

        public void Delete(int id)
        {
            if (id <= default(int))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ExecuteNonQuery(@$"DELETE FROM {_databaseName}.{_tableName} WHERE id = {id};");
        }

        public IEnumerable<Book> List()
        {
            return ExecuteReader(@$"SELECT * FROM {_databaseName}.{_tableName};");
        }

        public Book Details(int id)
        {
            if (id <= default(int))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return ExecuteReader(@$"SELECT * FROM {_databaseName}.{_tableName} WHERE id = {id};").FirstOrDefault();
        }

        private IEnumerable<Book> ExecuteReader(string commandText)
        {
            var books = new List<Book>();

            var command = _connection.CreateCommand();

            command.CommandText = commandText;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    books.Add(new Book
                    (
                        reader.GetInt32(0),     // Id
                        reader.GetString(1),    // Name
                        reader.GetString(2),    // Author
                        reader.GetInt32(3)      // Year
                    ));
                }
            }

            return books;
        }

        private void ExecuteNonQuery(string commandText)
        {
            var command = _connection.CreateCommand();

            command.CommandText = commandText;
            command.ExecuteNonQuery();
        }
    }
}
