using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDAL
{
    public class Library
    {
        // File Paths
        private static string _bookPath = "Book.txt";
        private static string _transactionPath = "Transactions.txt";
        private static string _borrowersPath = "Borrowers.txt";

        #region Library Main Methods
        public static void AddBook(Book book)
        {
            using (FileStream fin = new FileStream(_bookPath, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fin))
                {
                    string data = $"{book.BookID}|{book.Title}|{book.Author}|{book.Genre}|{book.IsAvailable}";
                    sw.WriteLine(data);
                }
            }
        }
        public void RemoveBook(int bookId)
        {
            List<Book> books = getAllBooks();
            using (var fin = new FileStream(_bookPath, FileMode.Truncate)) { }
            bool flag = true;
            foreach (Book book in books)
            {
                if (book.BookID == bookId)
                {
                    flag = false;
                    continue;
                }
                else
                {
                    AddBook(book);
                }
            }

            if (flag)
            {
                throw new ArgumentException("Book Not Found!");
            }
        }
        public List<Book> getAllBooks()
        {
            List<Book> books = new List<Book>();
            using (FileStream fin = new FileStream(_bookPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sw = new StreamReader(fin))
                {
                    string book;
                    while ((book = sw.ReadLine()) != null)
                    {
                        var tokens = book.Split('|');
                        books.Add(new Book
                        {
                            BookID = int.Parse(tokens[0]),
                            Title = tokens[1],
                            Author = tokens[2],
                            Genre = tokens[3],
                            IsAvailable = Convert.ToBoolean(tokens[4])
                        });
                    }
                }
            }
            return books;
        }
        public Book GetBookById(int bookId)
        {
            Book retBook;
            using (FileStream fin = new FileStream(_bookPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sw = new StreamReader(fin))
                {
                    string book;
                    while ((book = sw.ReadLine()) != null)
                    {
                        var tokens = book.Split('|');
                        if (int.Parse(tokens[0]) == bookId)
                        {
                            retBook = new Book
                            {
                                BookID = int.Parse(tokens[0]),
                                Title = tokens[1],
                                Author = tokens[2],
                                Genre = tokens[3],
                                IsAvailable = Convert.ToBoolean(tokens[4])
                            };
                            return retBook;
                        }
                    }
                }
            }
            return null;
        }
        public void UpdateBook(Book book)
        {
            RemoveBook(book.BookID);
            AddBook(book);
        }
        public List<Book> SearchBooks(string query)
        {
            query = query.ToLower().Trim();
            List<Book> books = new List<Book>();
            using (FileStream fin = new FileStream(_bookPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sw = new StreamReader(fin))
                {
                    string book;
                    while ((book = sw.ReadLine()) != null)
                    {
                        var tokens = book.Split('|');
                        if (tokens[1].ToLower() == query || tokens[2].ToLower() == query || tokens[3].ToLower() == query)
                        {
                            books.Add(new Book
                            {
                                BookID = int.Parse(tokens[0]),
                                Title = tokens[1],
                                Author = tokens[2],
                                Genre = tokens[3],
                                IsAvailable = Convert.ToBoolean(tokens[4])
                            });
                        }
                    }
                }
            }
            return books;
        }
        public void RegisterBorrower(Borrower borrower)
        {
            using (FileStream fin = new FileStream(_borrowersPath, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fin))
                {
                    string data = $"{borrower.BorrowerId}|{borrower.Name}|{borrower.Email}";
                    sw.WriteLine(data);
                }
            }
        }
        public void DeleteBorrower(int borrowerId)
        {
            List<Borrower> borrowers = getAllBorrowers();
            using (var fin = new FileStream(_borrowersPath, FileMode.Truncate)) { }
            foreach (var borrower in borrowers)
            {
                if (borrowerId == Convert.ToInt32(borrower.BorrowerId))
                {
                    continue;
                }
                else
                {
                    RegisterBorrower(borrower);
                }
            }
        }
        public void UpdateBorrower(Borrower borrower)
        {
            DeleteBorrower(borrower.BorrowerId);
            RegisterBorrower(borrower);
        }
        public void RecordTransaction(Transaction transaction)
        {
            using (FileStream fin = new FileStream(_transactionPath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fin))
                {
                    string data = $"{transaction.TransactionID}|{transaction.BookId}|{transaction.BorrowerId}|{transaction.Date}|{transaction.isBorrowed}";
                    sw.WriteLine(data);
                }
            }
        }
        public List<Transaction> GetBorrowedBooksByBorrower(int borrowerId)
        {
            List<Transaction> transactions = new List<Transaction>();
            using (FileStream fin = new FileStream(_transactionPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sw = new StreamReader(fin))
                {
                    string transaction;
                    while ((transaction = sw.ReadLine()) != null)
                    {
                        var tokens = transaction.Split('|');
                        if (int.Parse(tokens[2]) == borrowerId && Convert.ToBoolean(tokens[4]) == true)
                        {
                            int id = int.Parse(tokens[1]);
                            Book book = GetBookById(id);
                            if (book.IsAvailable == false)
                            {
                                transactions.Add(new Transaction
                                {
                                    TransactionID = int.Parse(tokens[0]),
                                    BookId = id,
                                    BorrowerId = int.Parse(tokens[2]),
                                    Date = Convert.ToDateTime(tokens[3]),
                                    isBorrowed = Convert.ToBoolean(tokens[4])
                                });
                            }
                        }
                    }
                }
            }
            return transactions;
        }
        #endregion

        #region Helper Functions
        public List<Borrower> getAllBorrowers()
        {
            List<Borrower> borrowers = new List<Borrower>();
            using (FileStream fin = new FileStream(_borrowersPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sw = new StreamReader(fin))
                {
                    string borrower;
                    while ((borrower = sw.ReadLine()) != null)
                    {
                        var parts = borrower.Split('|');
                        borrowers.Add(new Borrower
                        {
                            BorrowerId = Convert.ToInt32(parts[0]),
                            Email = parts[2],
                            Name = parts[1]
                        });
                    }
                }
            }
            return borrowers;
        }
        public HashSet<int> LoadBooksIds()
        {
            HashSet<int> BookIds = new HashSet<int>();
            using (var fin = new FileStream(_bookPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (var sw = new StreamReader(fin))
                {
                    string line;
                    while ((line = sw.ReadLine()) != null)
                    {
                        var tokens = line.Split('|');
                        BookIds.Add(Convert.ToInt32(tokens[0]));
                    }
                }
            }
            return BookIds;
        }
        public HashSet<int> LoadBorrowersIds()
        {
            HashSet<int> BorrowersIds = new HashSet<int>();
            using (var fin = new FileStream(_borrowersPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (var sw = new StreamReader(fin))
                {
                    string line;
                    while ((line = sw.ReadLine()) != null)
                    {
                        var tokens = line.Split('|');
                        BorrowersIds.Add(Convert.ToInt32(tokens[0]));
                    }
                }
            }
            return BorrowersIds;
        }
        public HashSet<int> LoadTransactionIds()
        {
            HashSet<int> TransactionIds = new HashSet<int>();
            using (var fin = new FileStream(_transactionPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (var sw = new StreamReader(fin))
                {
                    string line;
                    while ((line = sw.ReadLine()) != null)
                    {
                        var tokens = line.Split('|');
                        TransactionIds.Add(Convert.ToInt32(tokens[0]));
                    }
                }
            }
            return TransactionIds;
        }
        public bool isBookBorrowed(int id)
        {
            Book book = GetBookById(id);
            return !book.IsAvailable;
        }
        public HashSet<string> LoadEmails()
        {
            HashSet<string> emails = new HashSet<string>();
            using (var fin = new FileStream(_borrowersPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fin))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        var tokens = line.Split('|');
                        emails.Add(tokens[2]);
                    }
                }
            }
            return emails;
        }
        public void makeBookBorrow(int id)
        {
            Book book = GetBookById(id);
            book.IsAvailable = false;
            UpdateBook(book);
        }
        public void RemoveTransaction(int id)
        {
            using (var fin = new FileStream(_transactionPath, FileMode.OpenOrCreate, FileAccess.Read))
            using (var sr = new StreamReader(fin))
            using (var fout = new FileStream("temp.txt", FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fout))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    var tokens = line.Split('|');
                    if (tokens[0] != id.ToString())
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            File.Delete(_transactionPath);
            File.Move("temp.txt", _transactionPath);
        }
        public bool ReturnBook(int BookId, int BorrowId)
        {
            int id = 0;
            bool retVal = false;
            using (var filep = new FileStream(_transactionPath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (var sr = new StreamReader(filep))
                {
                    string line = "";
                    bool flag = true;
                    while (flag && (line = sr.ReadLine()) != null)
                    {
                        var tokens = line.Split('|');
                        if (tokens[1] == BookId.ToString() && tokens[2] == BorrowId.ToString() && Convert.ToBoolean(tokens[4]) == true)
                        {
                            id = Convert.ToInt32(tokens[0]);
                            flag = false;
                            retVal = true;
                        }
                    }
                }
            }
            if (retVal)
            {
                RemoveTransaction(id);
                RecordTransaction(new Transaction { TransactionID = id, BookId = BookId, BorrowerId = BorrowId, Date = DateTime.Now, isBorrowed = false });
            }
            return retVal;
        }
        public string GetEmail(int borrowerId)
        {
            string email = "";
            bool flag = true;
            using (var fin = new FileStream(_borrowersPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (var sr = new StreamReader(fin))
                {
                    string line = "";
                    while (flag && (line = sr.ReadLine()) != null)
                    {
                        var tokens = line.Split('|');
                        if (tokens[0] == borrowerId.ToString())
                        {
                            email = tokens[2];
                            flag = false;
                        }
                    }
                }
            }
            return email;
        }
        #endregion
    }
}