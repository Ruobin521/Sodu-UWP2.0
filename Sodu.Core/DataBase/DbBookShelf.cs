using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodu.Core.Entity;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace Sodu.Core.DataBase
{
    public class DbBookShelf
    {
        public static List<Book> GetBooks(string path)
        {
            var list = new List<Book>();
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<Book>();

                    db.RunInTransaction(() =>
                    {
                        try
                        {
                            var temp = from m in db.Table<Book>()
                                       select m;

                            var enumerable = temp as IList<Book> ?? temp.ToList();
                            list = !enumerable.Any() ? null : enumerable.ToList().OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            list = null;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                list = null;
            }
            return list;
        }

        public static bool InsertOrUpdateBook(string path, Book book)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<Book>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<Book>()
                                    where m.BookId == book.BookId
                                    select m
                                ).FirstOrDefault();
                        if (temp == null)
                        {
                            db.Insert(book);
                        }
                        else
                        {
                            book.Id = temp.Id;
                            db.Update(book);
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

        public static bool InsertOrUpdateBooks(string path, List<Book> books)
        {
            var result = true;
            ClearBooks(path);
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<Book>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        db.InsertAll(books);
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                });
            }
            return result;
        }
        public static bool ClearBooks(string path)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<Book>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        db.DeleteAll<Book>();
                    }
                    catch (Exception)
                    {
                        result = false;
                    }

                });
            }
            return result;
        }

        public static bool RemoveBook(string path, Book book)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<Book>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<Book>()
                                    where m.BookId == book.BookId
                                    select m
                                ).FirstOrDefault();
                        if (temp != null)
                        {
                            db.Delete(temp);
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
