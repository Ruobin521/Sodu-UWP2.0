using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sodu.Core.Entity;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace Sodu.Core.DataBase
{
    public class DbLocalBook
    {
        public static List<Book> GetBooks(string path)
        {
            var list = new List<Book>();
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<LocalBook>();

                    db.RunInTransaction(() =>
                    {
                        try
                        {
                            var enumerable = db.Table<LocalBook>()?.ToList();
                            if (enumerable == null || enumerable.Count == 0)
                            {
                                return;
                            }

                            enumerable = enumerable.ToList().OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();

                            foreach (var localBook in enumerable)
                            {
                                if (!string.IsNullOrEmpty(localBook.BookJson))
                                {
                                    var book = JsonConvert.DeserializeObject<Book>(localBook.BookJson);
                                    list.Add(book);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                            list = null;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                list = null;
            }
            return list;
        }

        public static int GetBooksCount(string path)
        {
            int count = 0;
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<LocalBook>();

                    db.RunInTransaction(() =>
                    {
                        try
                        {
                            var enumerable = db.Table<LocalBook>()?.ToList();
                            if (enumerable == null || enumerable.Count == 0)
                            {
                                return;
                            }

                            count = enumerable.Count;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                            count = 0;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                count = 0;
            }
            return count;
        }

        public static bool InsertOrUpdatBook(string path, Book book)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.BusyTimeout= TimeSpan.FromSeconds(5);
                db.CreateTable<LocalBook>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<LocalBook>()
                                    where m.BookId == book.BookId
                                    select m
                                ).FirstOrDefault();
                        if (temp == null)
                        {
                            var schema = new LocalBook()
                            {
                                BookId = book.BookId,
                                BookJson = JsonConvert.SerializeObject(book),
                                UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm")
                            };
                            db.Insert(schema);
                        }
                        else
                        {
                            temp.BookJson = JsonConvert.SerializeObject(book);
                            temp.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm");
                            db.Update(temp);
                        }
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                });
            }
            return result;
        }

        public static bool CheckBookExist(string path, string bookId)
        {
            bool result = false;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<LocalBook>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<LocalBook>()
                                    where m.BookId == bookId
                                    select m
                                ).FirstOrDefault();
                        if (temp != null)
                        {
                            result = true;
                        }
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                });
            }
            return result;
        }

        public static bool InsertOrUpdateBookCatalogs(string path, List<BookCatalog> catalogs)
        {
            var result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookCatalog>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        foreach (var catalog in catalogs)
                        {
                            db.Insert(catalog);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                        result = false;
                    }
                });
            }
            return result;
        }

        public static bool DeleteBookCatalogsByBookId(string path, string bookId)
        {
            var result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookCatalog>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var catalogs = (from m in db.Table<BookCatalog>()
                                        where m.BookId == bookId
                                        select m);

                        var count = db.Delete(catalogs);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                        result = false;
                    }
                });
            }
            return result;
        }


        public static List<BookCatalog> SelectBookCatalogsByBookId(string path, string bookId)
        {
            var result = new List<BookCatalog>();
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookCatalog>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var catalogs = (from m in db.Table<BookCatalog>()
                                        where m.BookId == bookId
                                        select m);

                        result = catalogs.OrderBy(p => p.Index).ToList();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                        result = null;
                    }
                });
            }
            return result;
        }


        public static BookCatalog SelectBookCatalogById(string path, string bookId, string catalogUrl)
        {
            BookCatalog result = null;
            Debug.WriteLine("进入数据库");
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                try
                {
                    db.BusyTimeout = TimeSpan.FromMilliseconds(1);
                    db.CreateTable<BookCatalog>();
                    db.RunInTransaction(() =>
                    {
                        try
                        {
                            var catalog = (from m in db.Table<BookCatalog>()
                                           where m.BookId == bookId && m.CatalogUrl == catalogUrl
                                           select m).FirstOrDefault();

                            result = catalog;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                            result = null;
                        }
                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                }
               
            }
            return result;
        }

        public static bool DeleteBook(string path, string bookId)
        {
            var result = true;

            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookCatalog>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var book = (from m in db.Table<LocalBook>()
                                    where m.BookId == bookId
                                    select m).FirstOrDefault();

                        if (book != null)
                        {
                            db.Delete(book);
                        }

                        var catalogs = (from m in db.Table<BookCatalog>()
                                        where m.BookId == bookId
                                        select m);

                        if (!catalogs.Any())
                        {
                            return;
                        }

                        foreach (var bookCatalog in catalogs)
                        {
                            db.Delete(bookCatalog);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                        result = false;
                    }
                });
            }
            return result;
        }

    }
}
