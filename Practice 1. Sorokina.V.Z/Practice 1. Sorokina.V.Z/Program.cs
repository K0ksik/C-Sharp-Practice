using System;
using System.IO;
using System.Collections.Generic;

class Library
{
    private List<Book> books = new List<Book>();
    private List<User> users = new List<User>();

    public void LoadData()
    {
        if (File.Exists("Книги.txt"))
        {
            using (StreamReader reader = new StreamReader("Книги.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    books.Add(new Book(parts[0], parts[1], parts[2] == "доступна"));
                }
            }
        }

        if (File.Exists("Пользователи.txt"))
        {
            using (StreamReader reader = new StreamReader("Пользователи.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    users.Add(new User(parts[0], parts[1].Split(';')));
                }
            }
        }
    }

    public void SaveData()
    {
        using (StreamWriter writer = new StreamWriter("Книги.txt"))
        {
            foreach (var book in books)
            {
                writer.WriteLine(ConvertBookToString(book));
            }
        }

        using (StreamWriter writer = new StreamWriter("Пользователи.txt"))
        {
            foreach (var user in users)
            {
                writer.WriteLine(ConvertUserToString(user));
            }
        }
    }

    private string ConvertBookToString(Book book)
    {
        string availability = book.IsAvailable ? "доступна" : "выдана";
        return $"{book.Title},{book.Author},{availability}";
    }

    private string ConvertUserToString(User user)
    {
        if (user.BorrowedBooks == null || user.BorrowedBooks.Count == 0)
        {
            return $"{user.Name},";
        }

        string borrowedBooks = "";
        for (int i = 0; i < user.BorrowedBooks.Count; i++)
        {
            borrowedBooks += user.BorrowedBooks[i];
            if (i < user.BorrowedBooks.Count - 1)
            {
                borrowedBooks += ";";
            }
        }
        return $"{user.Name},{borrowedBooks}";
    }


    public void AddBook(Book book)
    {
        books.Add(book);
    }

    public void RemoveBook(string title)
    {
        for (int i = books.Count - 1; i >= 0; i--)
        {
            if (books[i].Title == title)
            {
                books.RemoveAt(i);
            }
        }
    }

    public List<Book> GetAvailableBooks()
    {
        List<Book> availableBooks = new List<Book>();
        foreach (var book in books)
        {
            if (book.IsAvailable)
            {
                availableBooks.Add(book);
            }
        }
        return availableBooks;
    }

    public void RegisterUser(User user)
    {
        users.Add(user);
    }

    public List<User> GetAllUsers()
    {
        return users;
    }

    public List<Book> GetAllBooks()
    {
        return books;
    }
}

class Book
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public bool IsAvailable { get; set; }

    public Book(string title, string author, bool isAvailable)
    {
        Title = title;
        Author = author;
        IsAvailable = isAvailable;
    }
}

abstract class Person
{
    public string Name { get; private set; }

    protected Person(string name)
    {
        Name = name;
    }

    public abstract void Menu(Library library);
}

class User : Person
{
    public List<string> BorrowedBooks { get; private set; }

    public User(string name, string[] borrowedBooks = null) : base(name)
    {
        List<string> BorrowedBooks;

        if (borrowedBooks != null)
        {
            BorrowedBooks = new List<string>(borrowedBooks);
        }
        else
        {
            BorrowedBooks = new List<string>();
        }

    }

    public override void Menu(Library library)
    {
        while (true)
        {
            Console.WriteLine("1. Просмотреть книги\n2. Взять книгу\n3. Вернуть книгу\n4. Список взятых книг\n5. Выйти");
            switch (Console.ReadLine())
            {
                case "1":
                {
                    ViewAvailableBooks(library); 
                    break;
                }
                case "2":
                {
                    BorrowBook(library);
                    break;
                }
                    
                case "3":
                {
                    ReturnBook(library);
                    break;
                }
                case "4":
                {
                    ViewBorrowedBooks(); 
                    break;
                }
                case "5":
                {
                    return; 
                }

            }
        }
    }

    private void ViewAvailableBooks(Library library)
    {
        foreach (Book book in library.GetAvailableBooks())
        {
            Console.WriteLine($"Книга: {book.Title}, Автор: {book.Author}");
        }
    }
    private void BorrowBook(Library library)
    {
        Console.WriteLine("Введите название книги:");
        string title = Console.ReadLine();
        Book book = null;

        List<Book> allBooks = library?.GetAllBooks();
        if (allBooks == null)
        {
            Console.WriteLine("Ошибка: книги не найдены");
            return;
        }

        for (int i = 0; i < allBooks.Count; i++)
        {
            Book bk = allBooks[i];
            if (bk.Title == title && bk.IsAvailable)
            {
                book = bk;
                break;
            }
        }

        if (book != null)
        {
            book.IsAvailable = false;

            if (BorrowedBooks == null)
            {
                BorrowedBooks = new List<string>();
            }

            BorrowedBooks.Add(title);
            Console.WriteLine("Книга взята");
        }
        else
        {
            Console.WriteLine("Книга недоступна");
        }

    }
    private void ReturnBook(Library library)
    {
        Console.WriteLine("Название книги:");
        string title = Console.ReadLine();
        Book book = null;

        List<Book> allBooks = library.GetAllBooks();
        for (int i = 0; i < allBooks.Count; i++)
        {
            Book bk = allBooks[i];
            if (bk.Title == title && !bk.IsAvailable)
            {
                book = bk;
                break;
            }
        }

        if (book != null && BorrowedBooks.Contains(title))
        {
            book.IsAvailable = true;
            BorrowedBooks.Remove(title);
            Console.WriteLine("Книга возвращена");
        }
        else
        {
            Console.WriteLine("Не удалось вернуть книгу");
        }
    }

    private void ViewBorrowedBooks()
    {
        foreach (string book in BorrowedBooks)
        {
            Console.WriteLine(book);
        }
    }
}

class Librarian : Person
{
    public Librarian(string name) : base(name) { }

    public override void Menu(Library library)
    {
        while (true)
        {
            Console.WriteLine("1. Добавить книгу\n2. Удалить книгу\n3. Зарегистрировать пользователя\n4. Список пользователей\n5. Список книг\n6. Выйти");
            switch (Console.ReadLine())
            {
                case "1":
                    {
                        AddBook(library);
                        break;
                    }
                case "2":
                    {
                        RemoveBook(library);
                        break;
                    }
                case "3":
                    {
                        RegisterUser(library);
                        break;
                    }
                case "4":
                    {
                        ViewAllUsers(library);
                        break;
                    }
                case "5":
                    {
                        ViewAllBooks(library);
                        break;
                    }
                case "6":
                    {
                        return;
                    }
            }
        }
    }

    private void AddBook(Library library)
    {
        Console.WriteLine("Введите название книги:");
        string title = Console.ReadLine();
        Console.WriteLine("Введите автора:");
        string author = Console.ReadLine();
        library.AddBook(new Book(title, author, true));
        Console.WriteLine("Книга добавлена");
    }

    private void RemoveBook(Library library)
    {
        Console.WriteLine("Название книги:");
        string title = Console.ReadLine();
        library.RemoveBook(title);
        Console.WriteLine("Книга удалена");
    }

    private void RegisterUser(Library library)
    {
        Console.WriteLine("Введите имя пользователя:");
        string name = Console.ReadLine();
        library.RegisterUser(new User(name));
        Console.WriteLine("Пользователь зарегистрирован");
    }

    private void ViewAllUsers(Library library)
    {
        foreach (User user in library.GetAllUsers())
        {
            Console.WriteLine(user.Name);
        }
    }

    private void ViewAllBooks(Library library)
    {
        foreach (Book book in library.GetAllBooks())
        {
            string availability;
            if (book.IsAvailable)
            {
                availability = "доступна";
            }
            else
            {
                availability = "выдана";
            }
            Console.WriteLine($"{book.Title}, Автор: {book.Author} - {availability}");
        }
    }

}
class Program
{
    static void Main(string[] args)
    {
        Library library = new Library();
        library.LoadData();

        Console.WriteLine("Выберите роль:\n1. Библиотекарь\n2. Пользователь");
        string choice = Console.ReadLine();

        Person person;
        if (choice == "1")
        {
            Console.WriteLine("Введите имя библиотекаря:");
            string librarianName = Console.ReadLine();
            person = new Librarian(librarianName);
        }
        else
        {
            person = new User(GetUserName());
        }
        person.Menu(library);

        library.SaveData();
    }
    static string GetUserName()
    {
        Console.WriteLine("Введите имя:");
        return Console.ReadLine();
    }
}

