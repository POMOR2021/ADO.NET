using Microsoft.Extensions.Configuration;
using System;

class Program
{
    static Database db = new Database();
    static User us = new User();

    static void Main()
    {
        while (true)
        {
            
            Console.WriteLine("\nМеню:");
            Console.WriteLine("-Регистрация");
            Console.WriteLine("+Вход\n\n");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Показать книги");
            Console.WriteLine("3. Продать книгу");
            Console.WriteLine("4. Поиск книг");
            Console.WriteLine("5. Редактор книг");
            Console.WriteLine("6. Списать книгу");
            Console.WriteLine("7. Добавить книгу в акцию");
            Console.WriteLine("8. Забронировать книгу для покупателя");
            Console.WriteLine("9. Показать забронированные книги");
            Console.WriteLine("10. Показать топ продоваемых книг");
            Console.WriteLine("11. Показать топ авторов");
            Console.WriteLine("12. Показать топ жанров");
            Console.WriteLine("! - Выйти");

            Console.Write("Выберите действие: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "-":
                    Console.Write("Логин: ");
                    string user = Console.ReadLine(); 
                    Console.Write("Пароль: ");
                    string pass = Console.ReadLine(); 
                    us.Register(user, pass); 
                    break;
                case "+": 
                    Console.Write("Логин: "); 
                    string login = Console.ReadLine();
                    Console.Write("Пароль: "); 
                    string password = Console.ReadLine(); 
                    us.Login(login, password);
                    break;
                case "1":
                    AddBook();
                    break;
                case "2":
                    db.ShowBooks();
                    break;
                case "3":
                    SellBook();
                    break;
                case "4":
                    SearchBook();
                    break;
                case "5":
                    EditBook();
                    break;
                case "6":
                    Console.Write("Введите ID книги для списания: ");
                    int writeOffId = int.Parse(Console.ReadLine());
                    db.SpecBook(writeOffId);
                    break;
                case "7": 
                    Console.Write("Введите жанр книг для акции: "); 
                    string genre = Console.ReadLine(); 
                    Console.Write("Введите процент скидки: ");
                    decimal discount = decimal.Parse(Console.ReadLine());
                    db.Discount(genre, discount);
                    break;
                case "8":
                    Console.Write("Введите логин покупателя: ");
                    string reserveUser = Console.ReadLine();
                    Console.Write("Введите ID книги для откладки: ");
                    int reserveBookId = int.Parse(Console.ReadLine());
                    db.ReserveBook(reserveBookId, reserveUser);
                    break;

                case "9":
                    Console.Write("Введите ваш логин: ");
                    string userReservations = Console.ReadLine();
                    db.ViewReserv(userReservations);
                    break;
                case "10":
                    Console.Write("Введите период (день, неделя, месяц, год): ");
                    string periodBooks = Console.ReadLine();
                    db.ViewTopSellB(periodBooks);
                    break;

                case "11":
                    Console.Write("Введите период (день, неделя, месяц, год): ");
                    string periodAuthors = Console.ReadLine();
                    db.ViewTAuth(periodAuthors);
                    break;

                case "12":
                    Console.Write("Введите период (день, неделя, месяц, год): ");
                    string periodGenres = Console.ReadLine();
                    db.ViewTGenr(periodGenres);
                    break;
                case "!":
                    return;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
        }
    }

    static void AddBook()
    {
        Console.Write("Название: ");
        string title = Console.ReadLine();
        Console.Write("Автор: ");
        string author = Console.ReadLine();
        Console.Write("Издательство: ");
        string publisher = Console.ReadLine();
        Console.Write("Страницы: ");
        int pages = int.Parse(Console.ReadLine());
        Console.Write("Жанр: ");
        string genre = Console.ReadLine();
        Console.Write("Год издания: ");
        int year = int.Parse(Console.ReadLine());
        Console.Write("Себестоимость(в руб.): ");
        decimal cost = decimal.Parse(Console.ReadLine());
        Console.Write("Цена продажи(в руб.): ");
        decimal price = decimal.Parse(Console.ReadLine());
        Console.Write("Продолжение другой книги? (1 - да, 0 - нет): ");
        bool isContinuation = Console.ReadLine() == "1";

        db.AddBook(title, author, publisher, pages, genre, year, cost, price, isContinuation);
        Console.WriteLine("Книга добавлена!");
    }

    static void SellBook()
    {
        Console.Write("Введите ID книги: ");
        int bookId = int.Parse(Console.ReadLine());
        Console.Write("Введите количество: ");
        int quantity = int.Parse(Console.ReadLine());

        db.SellBook(bookId, quantity);
        Console.WriteLine("Книга продана!");
    }

    static void SearchBook()
    {
        Console.Write("Введите название, автора или жанр: ");
        string keyword = Console.ReadLine();
        db.SearchBooks(keyword);
    }
    static void EditBook()
    {
        Console.Write("Введите ID книги, которую хотите отредактировать: ");
        if (int.TryParse(Console.ReadLine(), out int bookId))
        {
            Console.Write("Введите новое название книги: ");
            string newTitle = Console.ReadLine();

            Console.Write("Введите нового автора: ");
            string newAuthor = Console.ReadLine();

            Console.Write("Введите новое издательство: ");
            string newPublisher = Console.ReadLine();

            Console.Write("Введите количество страниц: ");
            int newPages = int.Parse(Console.ReadLine());

            Console.Write("Введите новый жанр: ");
            string newGenre = Console.ReadLine();

            Console.Write("Введите год издания: ");
            int newYear = int.Parse(Console.ReadLine());

            Console.Write("Введите себестоимость книги(в руб.): ");
            decimal newCostPrice = decimal.Parse(Console.ReadLine());

            Console.Write("Введите цену продажи(в руб.): ");
            decimal newSalePrice = decimal.Parse(Console.ReadLine());

            Console.Write("Введите ID книги, к которой эта книга является продолжением или 0, если она не является продолжением: ");
            int newSequelTo = int.Parse(Console.ReadLine());
            int? sequelTo = newSequelTo > 0 ? newSequelTo : null;

            db.EditBook(bookId, newTitle, newAuthor, newPublisher, newPages, newGenre, newYear, newCostPrice, newSalePrice, sequelTo);
        }
        else
        {
            Console.WriteLine("Ошибка: введён некорректный ID.");
        }
    }
}
