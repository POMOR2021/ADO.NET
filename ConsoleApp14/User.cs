using System;
using System.Data.SqlClient;
using ConsoleApp14;
class User
{
    private readonly string ConnectionString = ConsoleApp14.StringConnection.ConnectionString;
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public bool Register(string username, string password)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, 'user')";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Регистрация успешна!");
                return true;
            }
            catch
            {
                Console.WriteLine("Ошибка: Логин занят.");
                return false;
            }
        }
    }

    public bool Login(string username, string password)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            string query = "SELECT Role FROM Users WHERE Username = @Username AND Password = @Password";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine("Вход успешен!");
                return true;
            }

            Console.WriteLine("Неверные данные.");
            return false;
        }
    }
}
