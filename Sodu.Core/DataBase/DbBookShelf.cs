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
    public class DbBookShelf
    {
        public static List<Book> GetBooks(string path, string userId)
        {
            List<Book> list = new List<Book>();
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookShelfSchema>();
                    db.RunInTransaction(() =>
                    {
                        try
                        {
                            var temp = from m in db.Table<BookShelfSchema>()
                                       where m.UserId == userId
                                       select m;

                            var enumerable = temp as IList<BookShelfSchema> ?? temp.ToList();
                            if (enumerable != null && enumerable.Count > 0)
                            {
                                foreach (var bookShelfSchema in enumerable)
                                {
                                    if (string.IsNullOrEmpty(bookShelfSchema.BookJson))
                                    {
                                        continue;
                                    }
                                    var book = JsonConvert.DeserializeObject<Book>(bookShelfSchema.BookJson);

                                    if (book != null)
                                    {
                                        list.Add(book);
                                    }
                                }
                            }
                            list = list.OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();
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

        public static bool InsertOrUpdateBook(string path, Book book, string userId)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookShelfSchema>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<BookShelfSchema>()
                                    where m.BookId == book.BookId && m.UserId == userId
                                    select m
                                ).FirstOrDefault();
                        if (temp == null)
                        {
                            var schema = new BookShelfSchema()
                            {
                                BookId = book.BookId,
                                UserId = userId,
                                BookJson = JsonConvert.SerializeObject(book),
                            };
                            db.Insert(schema);
                        }
                        else
                        {
                            temp.BookJson = JsonConvert.SerializeObject(book);
                            temp.UserId = userId;
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

        public static bool InsertOrUpdateBooks(string path, List<Book> books, string userId)
        {
            var result = true;
            ClearBooks(path, userId);
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookShelfSchema>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        foreach (var book in books)
                        {
                            var schema = new BookShelfSchema()
                            {
                                BookId = book.BookId,
                                UserId = userId,
                                BookJson = JsonConvert.SerializeObject(book),
                            };
                            db.Insert(schema);
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
        public static bool ClearBooks(string path, string userId)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookShelfSchema>();
                db.RunInTransaction(() =>
                {
                    try
                    {

                        var sql = $"Delete from BookShelfSchema where Userid = ?";
                        var count = db.Execute(sql, new object[] { userId });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                        result = false;
                    }

                });
            }
            return result;
        }

        public static bool RemoveBook(string path, Book book, string userId)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookShelfSchema>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<BookShelfSchema>()
                                    where m.BookId == book.BookId && m.UserId == userId
                                    select m
                                ).FirstOrDefault();
                        if (temp != null)
                        {
                            var count = db.Delete(temp);
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
    }
}
