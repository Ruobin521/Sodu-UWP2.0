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
    public class DbHistory
    {
        public static List<Book> GetBooks(string path)
        {
            var list = new List<Book>();
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookHistorySchema>();

                    db.RunInTransaction(() =>
                    {
                        try
                        {
                            var enumerable = db.Table<BookHistorySchema>()?.ToList();
                            if (enumerable == null || enumerable.Count == 0)
                            {
                                return;
                            }

                            enumerable = enumerable.ToList().OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();

                            foreach (var history in enumerable)
                            {
                                if (!string.IsNullOrEmpty(history.BookJson))
                                {
                                    var book = JsonConvert.DeserializeObject<Book>(history.BookJson);
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
        public static bool InsertOrUpdatHistory(string path, Book book)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookHistorySchema>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<BookHistorySchema>()
                                    where m.BookId == book.BookId
                                    select m
                                ).FirstOrDefault();
                        if (temp == null)
                        {
                            var schema = new BookHistorySchema()
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

        public static bool ClearBooks(string path)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookHistorySchema>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        db.DeleteAll<BookHistorySchema>();
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
