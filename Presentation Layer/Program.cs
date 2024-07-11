#pragma warning disable CS8602, CS8604, CS8618

using LibraryDAL;
using System.Diagnostics;

public class PresentationLayerProgram
{
    // defining upper and lower limit for ID's
    public const int MIN = 1;
    public const int MAX = 99999;

    // creating an instance of our Library
    internal static Library library = new Library();

    internal static HashSet<int> Books;
    internal static HashSet<int> Borrowers;
    internal static HashSet<int> Transactions;
    internal static HashSet<string> Emails;

    static void Main()
    {
        // loadData function called to store all persistant data from Files
        LoadData();

        #region Main
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\t\tLibrary Console Application");
            Console.WriteLine("_____________________________________________________\n");
            displayMenu();
            Console.Write("\nEnter Choice: ");
            var choice = Console.ReadLine().Trim();

            switch (choice)
            {
                case "1":
                    AddBook();
                    break;
                case "2":
                    removeBook();
                    break;
                case "3":
                    UpdateBook();
                    break;
                case "4":
                    RegisterBorrower();
                    break;
                case "5":
                    UpdateBorrower();
                    break;
                case "6":
                    DeleteABorrower();
                    break;
                case "7":
                    BorrowBook();
                    break;
                case "8":
                    returnBook();
                    break;
                case "9":
                    SearchBooks();
                    break;
                case "10":
                    ViewAllBooks();
                    break;
                case "11":
                    BooksByBorrower();
                    break;
                case "12":
                    Console.WriteLine("\nApplication Exited Successfully ....\n");
                    return;
                default:
                    Console.WriteLine("Error: Invalid Input Entered!");
                    break;
            }
        }
        #endregion


    }

    #region Menu Functions
    public static void AddBook()
    {
        Console.Clear();
        Console.WriteLine("Enter Book Details:");
        Console.WriteLine("_____________________________________________________\n");

        string title = GetBookDetails("Title");
        string author = GetBookDetails("Author");
        string genre = GetBookDetails("Genre");

        // Generating Unique Id for the Type: Book
        int id = GenerateId("Book");
        Book book = new Book { Author = author, Genre = genre, Title = title, BookID = id };
        LibraryDAL.Library.AddBook(book);

        Console.WriteLine("\nBook Added Successfully!");
        Console.WriteLine("Press Enter to continue .... ");
        Console.Read();
    }
    public static void removeBook()
    {
        Console.Clear();
        Console.WriteLine("Removing Book");
        Console.WriteLine("_____________________________________________________\n");

        string input;
        bool flag = true;
        do
        {
            Console.Write("Enter Book Id (0 to exit): ");
            input = Console.ReadLine().Trim();

            if (input == "0")
            {
                return;
            }
            else if (ValidateBook(input))
            {
                flag = false;
            }

        } while (flag);

        var id = int.Parse(input);
        library.RemoveBook(id);
        Books.Remove(id);

        Console.WriteLine("\nBook Removed Succesfully!");
        Console.WriteLine("Press Enter to Continue ....");
        Console.Read();
    }
    public static void UpdateBook()
    {
        Console.Clear();
        Console.WriteLine("Updating Book");
        Console.WriteLine("_____________________________________________________\n");

        bool flag = true;
        string input;
        do
        {
            Console.Write("Enter Book Id(0 to exit): ");
            input = Console.ReadLine().Trim();

            if (input == "0")
            {
                return;
            }
            else if (ValidateBook(input))
            {
                flag = false;
            }

        } while (flag);

        var id = int.Parse(input);
        if (library.isBookBorrowed(id))
        {
            Console.WriteLine("\nError: Borrowed Book can't be Modified!");
        }
        else
        {
            Console.WriteLine("\nEnter New Details of the Book: ");
            string title = GetBookDetails("Title");
            string author = GetBookDetails("Author");
            string genre = GetBookDetails("Genre");

            library.UpdateBook(new Book { Title = title, Author = author, Genre = genre, BookID = id });
            Console.WriteLine("\nBook Updated Succesfully!");
        }
        Console.WriteLine("Press Enter to Continue ....");
        Console.Read();
    }
    public static void RegisterBorrower()
    {
        Console.Clear();
        Console.WriteLine("Enter Borrower Details:");
        Console.WriteLine("_____________________________________________________\n");

        int id = GenerateId("Borrower");
        string name = GetBorrowerName();
        string email = GetBorrowerEmail("");

        library.RegisterBorrower(new Borrower { BorrowerId = id, Name = name, Email = email });
        Console.WriteLine("\nBorrower Resgistered Successfully!");
        Console.WriteLine("Press Enter to Continue ....");
        Console.Read();
    }
    public static void DeleteABorrower()
    {
        Console.Clear();
        Console.WriteLine("Enter Details of Borrower to be Deleted: ");
        Console.WriteLine("_____________________________________________________\n");

        Console.Write("Enter Id: \t");
        string input = Console.ReadLine().Trim();
        if (ValidateBorrower(input, "delete"))
        {
            int id = int.Parse(input);
            library.DeleteBorrower(id);
            Borrowers.Remove(id);

            Console.WriteLine("\nBorrower Removed Successfully!");
        }
        Console.WriteLine("Press Enter to Continue ....");
        Console.Read();
    }
    public static void BorrowBook()
    {
        Console.Clear();
        Console.WriteLine("Borrow A Book: ");
        Console.WriteLine("_____________________________________________________\n");

        Console.Write("{0, -30}", "Enter Borrower Id:");
        string input = Console.ReadLine().Trim();

        if (ValidateBorrower(input, "Normal"))
        {
            int borrowerId = int.Parse(input);
            Console.Write("{0, -30}", "Enter Book Id:");
            input = Console.ReadLine().Trim();

            if (ValidateBook(input))
            {
                int BookId = int.Parse(input);

                // making the availablity of the book to unavailable
                library.makeBookBorrow(BookId);

                Transaction transaction = new Transaction
                {
                    TransactionID = GenerateId("Transaction"),
                    BookId = BookId,
                    BorrowerId = borrowerId,
                    isBorrowed = true,
                    Date = DateTime.Now
                };

                library.RecordTransaction(transaction);
                Books.Add(BookId);

                Console.WriteLine("\nBook Borrowed Successfully!");
            }
        }

        Console.WriteLine("Press Enter to Continue ....");
        Console.Read();
    }
    public static void ViewAllBooks()
    {
        Console.Clear();
        Console.WriteLine("\t\t\t\t\t Library");
        Console.WriteLine("------------------------------------------------------------------------------------------");

        List<Book> books = library.getAllBooks();
        if (books.Count == 0)
        {
            Console.WriteLine("No Books in Library!");
        }
        else
        {
            Console.WriteLine("{0,-10} {1,-30} {2,-20} {3,-15} {4,-10}", "BookId", "Title", "Author", "Genre", "Status");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            foreach (var book in books)
            {
                string available = book.IsAvailable ? "Available" : "Issued";
                Console.WriteLine("{0,-10} {1,-30} {2,-20} {3,-15} {4,-10}", book.BookID, book.Title, book.Author, book.Genre, available);
            }
        }

        Console.WriteLine("\nPress Enter to Continue ....");
        Console.ReadLine();
    }
    public static void BooksByBorrower()
    {
        Console.Clear();
        Console.WriteLine("Borrow A Book: ");
        Console.WriteLine("_____________________________________________________\n");

        Console.Write("Enter Borrower Id: \t");
        string input = Console.ReadLine().Trim();


        if (ValidateBorrower(input, "Normal"))
        {
            int id = int.Parse(input);
            List<Transaction> transactions = library.GetBorrowedBooksByBorrower(id);

            if (transactions.Count == 0)
            {
                Console.WriteLine("Borrower hasn't Borrowed Any Books!");
            }
            else
            {
                List<Book> books = new List<Book>();
                foreach (var transaction in transactions)
                {
                    books.Add(library.GetBookById(transaction.BookId));
                }

                Console.WriteLine("\nBorrowed Books:\n");
                Console.WriteLine("-----------------------------------------------------------------------------------------------");
                Console.WriteLine("{0,-10} {1,-50} {2,-20} {3,-20}", "Book Id", "Title", "Author", "Genre");
                Console.WriteLine("-----------------------------------------------------------------------------------------------");

                foreach (var book in books)
                {
                    Console.WriteLine("{0,-10} {1,-50} {2,-20} {3,-20}", book.BookID, book.Title, book.Author, book.Genre);
                }
            }
        }

        Console.WriteLine("\nPress Enter to Continue....");
        Console.Read();
    }
    public static void returnBook()
    {
        Console.Clear();
        Console.WriteLine("Return A Book: ");
        Console.WriteLine("_____________________________________________________\n");

        Console.Write("Enter Book ID: \t\t");
        string input = Console.ReadLine().Trim();

        if (ValidateReturn(input))
        {
            int BookId = Convert.ToInt32(input);
            Console.Write("Enter Borrower ID: \t");
            input = Console.ReadLine().Trim();
            if (ValidateBorrower(input, "None"))
            {
                if (library.ReturnBook(BookId, Convert.ToInt32(input)))
                {
                    Book book = library.GetBookById(BookId);
                    book.IsAvailable = true;
                    library.RemoveBook(BookId);
                    LibraryDAL.Library.AddBook(book);
                    Console.WriteLine("Book Returned Successfully!");
                }
                else
                {
                    Console.WriteLine("Error: No Such Record!");
                }
            }
        }

        Console.WriteLine("\nPress Enter to Continue ....");
        Console.Read();
    }
    public static void UpdateBorrower()
    {
        // if the borrower has borrowed any books then the details of the borrower
        // can't be updated
        Console.Clear();
        Console.WriteLine("Update A Borrower: ");
        Console.WriteLine("_____________________________________________________\n");

        Console.Write("{0, -20}", "Enter Borrower Id: ");

        string input = Console.ReadLine().Trim();

        if (ValidateBorrower(input, "update"))
        {
            int id = int.Parse(input);
            library.DeleteBorrower(id);
            string name = GetBorrowerName();

            string temp = library.GetEmail(id);
            string email = GetBorrowerEmail(temp);

            library.UpdateBorrower(new Borrower { BorrowerId = id, Email = email, Name = name });

            Emails = library.LoadEmails();
            Console.WriteLine("\nBorrower Updated Successfully!");
        }

        Console.WriteLine("Press Enter to Continue ....");
        Console.Read();
    }
    public static void SearchBooks()
    {
        Console.Clear();
        Console.WriteLine("Search by Title, Author OR Genre");
        Console.WriteLine("_____________________________________________________\n");

        Console.Write("Query: ");
        var query = Console.ReadLine();

        List<Book> books = library.SearchBooks(query);
        if (books.Count == 0)
        {
            Console.WriteLine("\nNo Books Found!");
        }
        else
        {
            Console.WriteLine("{0,-10} {1,-30} {2,-20} {3,-15} {4,-10}", "BookId", "Title", "Author", "Genre", "Status");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            foreach (var book in books)
            {
                string available = book.IsAvailable ? "Available" : "Issued";
                Console.WriteLine("{0,-10} {1,-30} {2,-20} {3,-15} {4,-10}", book.BookID, book.Title, book.Author, book.Genre, available);
            }
        }

        Console.WriteLine("\nPress Any  Key to Continue ....");
        Console.Read();
    }
    #endregion

    #region Helper Functions
    public static void LoadData()
    {
        Books = library.LoadBooksIds();
        Borrowers = library.LoadBorrowersIds();
        Transactions = library.LoadTransactionIds();
        Emails = library.LoadEmails();
    }
    public static void displayMenu()
    {
        Console.WriteLine("1.  Add a new book\n2.  Remove a book\n3.  Update a book\n4.  Register a new borrower\n5.  Update a borrower\r\n6.  Delete a borrower\n7.  Borrow a book\n8.  Return a book\n9.  Search for books by title, author, or genre\r\n10. View all books\n11. View borrowed books by a specific borrower\n12. Exit the application");
    }
    public static bool ValidateBook(string input)
    {
        Books = library.LoadBooksIds();
        bool retVal = true;
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Error: No Input Detected!");
            retVal = false;
        }
        else if (int.TryParse(input, out _))
        {
            int id = int.Parse(input);
            if (!Books.Contains(id))
            {
                Console.WriteLine("Error: Book doesn't exists in the Library");
                retVal = false;
            }
            else if (library.isBookBorrowed(id))
            {
                Console.WriteLine("Error: Book is Already Borrowed!");
                retVal = false;
            }
        }
        else
        {
            retVal = false;
            Console.WriteLine("Error: Invalid ID Entered!");
        }
        return retVal;
    }
    public static bool ValidateReturn(string input)
    {
        bool retVal = true;
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Error: No Input Detected!");
            retVal = false;
        }
        else if (int.TryParse(input, out _))
        {
            int id = int.Parse(input);
            if (!Books.Contains(id))
            {
                Console.WriteLine("Error: Book doesn't exists in the Library");
                retVal = false;
            }
            else if (!library.isBookBorrowed(id))
            {
                Console.WriteLine("Error: Book is Not Borrowed!");
                retVal = false;
            }
        }
        else
        {
            retVal = false;
            Console.WriteLine("Error: Invalid ID Entered!");
        }
        return retVal;
    }
    public static bool ValidateBorrower(string input, string type)
    {
        Borrowers = library.LoadBorrowersIds();

        bool retVal = true;
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Error: Borrower Id can't be NULL");
        }
        else if (int.TryParse(input, out _))
        {
            int id = int.Parse(input);
            if (!Borrowers.Contains(id))
            {
                Console.WriteLine("Error: Borrower isn't Registered!");
                retVal = false;
            }
            else
            {
                // checking if the borrower has borrowed any books
                // if yes then it can't be deleted or modified
                List<Transaction> transactions = library.GetBorrowedBooksByBorrower(id);
                if ((type == "delete" || type == "update") && transactions.Count > 0)
                {
                    Console.WriteLine("Error: Borrower cannot be Deleted until All books are returned!");
                    retVal = false;
                }
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid ID given!");
            retVal = false;
        }
        return retVal;
    }
    public static string GetBookDetails(string type)
    {
        string input = "";
        do
        {
            Console.Write("{0, -20}", $"{type} of Book:");
            input = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine($"Error: {type} of the Book Can't be Empty!");
            }
        } while (string.IsNullOrWhiteSpace(input));
        return input;
    }
    public static string GetBorrowerName()
    {
        string name = "";
        do
        {
            Console.Write("{0, -20}", "Enter Name:");
            name = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Error: Name Can't be Empty!");
            }

        } while (string.IsNullOrWhiteSpace(name));
        return name;
    }
    public static string GetBorrowerEmail(string exclude)
    {
        // The Exclude paramter is the email to exclude from validation checking
        string email;
        bool flag = true;
        do
        {
            Console.Write("{0, -20}", "Enter Email:");
            email = Console.ReadLine().Trim();
            flag = ValidateEmail(email, exclude);

        } while (!flag);

        return email;
    }
    public static bool ValidateFormat(string email)
    {
        HashSet<char> disallowedCharacters = new HashSet<char>
        {
            ' ', ',', ':', ';', '<', '>', '(', ')', '[', ']', '\\', '/', '"', '~'
        };
        bool retVal = true;
        int alphas = 0;
        foreach (var ch in email)
        {
            if (ch == '@')
            {
                alphas++;
            }
            else if (disallowedCharacters.Contains(ch))
            {
                retVal = false;
                break;
            }
        }
        if (alphas != 1)
        {
            retVal = false;
        }
        else
        {
            int i = email.IndexOf('@');
            string domain = email.Substring(i + 1);
            string local = email.Substring(0, i);

            var tokens = domain.Split('.');
            if (string.IsNullOrWhiteSpace(local) || string.IsNullOrWhiteSpace(local))
            {
                retVal = false;
            }
            else if (tokens.Count() > 3)
            {
                retVal = false;
            }
            else if (local.Contains(".."))
            {
                retVal = false;
            }
            else if (local.StartsWith(".") || local.EndsWith("."))
            {
                retVal = false;
            }

            if (retVal)
            {
                foreach (var token in tokens)
                {
                    if (token.Count() < 2)
                    {
                        retVal = false;
                        break;
                    }
                }
            }
        }
        return retVal;
    }
    public static bool ValidateEmail(string email, string exclude)
    {
        Emails = library.LoadEmails();
        bool retVal = true;
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Error: Email can't be Empty!");
            retVal = false;
        }
        else if (!ValidateFormat(email)) // if format is incorrect
        {
            Console.WriteLine("Error: Incorrect Format of Email");
            retVal = false;
        }
        else if (Emails.Contains(email) && email != exclude)
        {
            Console.WriteLine("Error: Email Already Registered!");
            retVal = false;
        }

        return retVal;
    }
    public static int GenerateId(string type)
    {
        int id = 0;
        Random random = new Random();
        bool flag = true;
        if (type == "Book")                     // for generating ID of Book
        {
            do
            {
                id = random.Next(MIN, MAX);
                if (!Books.Contains(id))
                {
                    flag = false;
                }
            } while (flag);
        }
        else if (type == "Borrower")            // for generating ID of Borrower
        {
            do
            {
                id = random.Next(MIN, MAX);
                if (!Borrowers.Contains(id))
                {
                    flag = false;
                }
            } while (flag);
        }
        else
        {                                       // for generating ID of Transaction
            do
            {
                id = random.Next(MIN, MAX);
                if (!Transactions.Contains(id))
                {
                    flag = false;
                }
            } while (flag);
        }
        return id;
    }
    #endregion
}