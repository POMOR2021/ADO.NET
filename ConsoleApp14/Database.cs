using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

class Database
{
    private readonly string ConnectionString = ConsoleApp14.StringConnection.ConnectionString;
    public void AddBook(string title, string author, string publisher, int pages, string genre, int year, decimal cost, decimal price, bool isContinuation)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = "INSERT INTO Books (Title, Author, Publisher, PageCount, Genre, YearPublished, CostPrice, SalePrice, IsContinuation) " +
                           "VALUES (@Title, @Author, @Publisher, @PageCount, @Genre, @YearPublished, @CostPrice, @SalePrice, @IsContinuation)";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Author", author);
            cmd.Parameters.AddWithValue("@Publisher", publisher);
            cmd.Parameters.AddWithValue("@PageCount", pages);
            cmd.Parameters.AddWithValue("@Genre", genre);
            cmd.Parameters.AddWithValue("@YearPublished", year);
            cmd.Parameters.AddWithValue("@CostPrice", cost);
            cmd.Parameters.AddWithValue("@SalePrice", price);
            cmd.Parameters.AddWithValue("@IsContinuation", isContinuation);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    public void ShowBooks()
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Books";
            SqlCommand cmd = new SqlCommand(query, connection);

            
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Название: {reader["Title"]}, Автор: {reader["Author"]}, Цена: {reader["SalePrice"]} руб.");
            }

            connection.Close();
        }
    }

    public void SellBook(int bookId, int quantity)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = "INSERT INTO Sales (BookId, Quantity) VALUES (@BookId, @Quantity)";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@BookId", bookId);
            cmd.Parameters.AddWithValue("@Quantity", quantity);

            
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }

    public void SearchBooks(string keyword)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Books " +
                           "WHERE Title LIKE @Keyword OR Author LIKE @Keyword OR Genre LIKE @Keyword";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

           
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Название: {reader["Title"]}, Автор: {reader["Author"]}");
            }

            connection.Close();
        }
    }
    public void EditBook(int bookId, string newTitle, string newAuthor, string newPublisher,
                         int newPages, string newGenre, int newYear,
                         decimal newCostPrice, decimal newSalePrice, int? newSequelTo)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            string query = @"UPDATE Books 
                             SET Title = @Title, Author = @Author, Publisher = @Publisher, 
                                 PageCount = @Pages, Genre = @Genre, YearPublished = @Year, 
                                 CostPrice = @CostPrice, SalePrice = @SalePrice, 
                                 IsContinuation = @SequelTo
                             WHERE Id = @BookId";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@BookId", bookId);
                cmd.Parameters.AddWithValue("@Title", newTitle);
                cmd.Parameters.AddWithValue("@Author", newAuthor);
                cmd.Parameters.AddWithValue("@Publisher", newPublisher);
                cmd.Parameters.AddWithValue("@Pages", newPages);
                cmd.Parameters.AddWithValue("@Genre", newGenre);
                cmd.Parameters.AddWithValue("@Year", newYear);
                cmd.Parameters.AddWithValue("@CostPrice", newCostPrice);
                cmd.Parameters.AddWithValue("@SalePrice", newSalePrice);
                cmd.Parameters.AddWithValue("@SequelTo", (object?)newSequelTo ?? DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                    Console.WriteLine("Книга успешно обновлена!");
                else
                    Console.WriteLine("Книга с таким ID отсутствует");
            }
        }
    }
    public void SpecBook(int id)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = "DELETE FROM Books WHERE Id = @Id";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0 ? "Книга удалена!" : "Книга отсутствует");
        }
    }
    public void Discount(string genre, decimal discountPercentage)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = @"UPDATE Books 
                             SET Discountprice = SalePrice - (SalePrice * @Discount / 100)
                             WHERE Genre = @Genre";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Discount", discountPercentage);
            command.Parameters.AddWithValue("@Genre", genre);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0 ? $"Акция применена! Книги жанра '{genre}' получили скидку {discountPercentage}%." : "Книги этого жанра отсутствуют");
        }
    }
    public void ReserveBook(int bookId, string username)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string checkBookQuery = "SELECT COUNT(*) FROM Books " +
                                    "WHERE Id = @BookId";
            var checkBookCommand = new SqlCommand(checkBookQuery, connection);
            checkBookCommand.Parameters.AddWithValue("@BookId", bookId);
            int bookExists = (int)checkBookCommand.ExecuteScalar();

            if (bookExists == 0)
            {
                Console.WriteLine("Книги нет ");
                return;
            }

            string checkReservationQuery = "SELECT COUNT(*) FROM Reservations WHERE BookId = @BookId";
            var checkReservationCommand = new SqlCommand(checkReservationQuery, connection);
            checkReservationCommand.Parameters.AddWithValue("@BookId", bookId);
            int alreadyReserved = (int)checkReservationCommand.ExecuteScalar();

            if (alreadyReserved > 0)
            {
                Console.WriteLine("Книга уже забронирована");
                return;
            }

            string insertQuery = @"INSERT INTO Reservations (BookId, Username) 
                                   VALUES (@BookId, @Username)";

            var insertCommand = new SqlCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@BookId", bookId);
            insertCommand.Parameters.AddWithValue("@Username", username);
            insertCommand.ExecuteNonQuery();

            Console.WriteLine($"Книга с ID {bookId} забронирована для {username}.");
        }
    }
    public void ViewReserv(string username)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = @"SELECT b.Id, b.Title, b.Author, r.ReservationDate FROM Reservations r
                             JOIN Books b ON r.BookId = b.Id
                             WHERE r.Username = @Username";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            var reader = command.ExecuteReader();

            Console.WriteLine($"Забронированные книги для {username}:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Id"]}. {reader["Title"]} - {reader["Author"]} (забронирована {reader["ReservationDate"]})");
            }
        }
    }
    public void ViewTopSellB(string period = "month")
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string dateCondition = period switch
            {
                "день" => "DATEDIFF(DAY, SaleDate, GETDATE()) = 0",
                "неделя" => "DATEDIFF(WEEK, SaleDate, GETDATE()) = 0",
                "месяц" => "DATEDIFF(MONTH, SaleDate, GETDATE()) = 0",
                "год" => "DATEDIFF(YEAR, SaleDate, GETDATE()) = 0",
                _ => "1=1"
            };

            string query = $@"SELECT TOP 10 b.Id, b.Title, b.Author, COUNT(s.Id) AS SalesCount FROM Sales s
                              JOIN Books b ON s.BookId = b.Id
                              WHERE {dateCondition}
                              GROUP BY b.Id, b.Title, b.Author
                              ORDER BY SalesCount DESC";

            var command = new SqlCommand(query, connection);
            var reader = command.ExecuteReader();

            Console.WriteLine($"\nТоп 10 продаваемых книг ({period}):");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Title"]} - {reader["Author"]} (Продано: {reader["SalesCount"]})");
            }
        }
    }
    public void ViewTAuth(string period = "month")
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string dateCondition = period switch
            {
                "день" => "DATEDIFF(DAY, SaleDate, GETDATE()) = 0",
                "неделя" => "DATEDIFF(WEEK, SaleDate, GETDATE()) = 0",
                "месяц" => "DATEDIFF(MONTH, SaleDate, GETDATE()) = 0",
                "год" => "DATEDIFF(YEAR, SaleDate, GETDATE()) = 0",
                _ => "1=1"
            };

            string query = $@"SELECT TOP 5 b.Author, COUNT(s.Id) AS SalesCount FROM Sales s
                              JOIN Books b ON s.BookId = b.Id
                              WHERE {dateCondition}
                              GROUP BY b.Author
                              ORDER BY SalesCount DESC";

            var command = new SqlCommand(query, connection);
            var reader = command.ExecuteReader();

            Console.WriteLine($"\nТоп 5 популярных авторов ({period}):");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Author"]} (Продано книг: {reader["SalesCount"]})");
            }
        }
    }
    public void ViewTGenr(string period = "month")
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string dateCondition = period switch
            {
                "день" => "DATEDIFF(DAY, SaleDate, GETDATE()) = 0",
                "неделя" => "DATEDIFF(WEEK, SaleDate, GETDATE()) = 0",
                "месяц" => "DATEDIFF(MONTH, SaleDate, GETDATE()) = 0",
                "год" => "DATEDIFF(YEAR, SaleDate, GETDATE()) = 0",
                _ => "1=1"
            };

            string query = $@"SELECT TOP 5 b.Genre, COUNT(s.Id) AS SalesCount FROM Sales s
                              JOIN Books b ON s.BookId = b.Id
                              WHERE {dateCondition}
                              GROUP BY b.Genre
                              ORDER BY SalesCount DESC";

            var command = new SqlCommand(query, connection);
            var reader = command.ExecuteReader();

            Console.WriteLine($"\nТоп 5 популярных жанров ({period}):");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Genre"]} (Продано книг: {reader["SalesCount"]})");
            }
        }
    }
}