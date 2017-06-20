using System;
using System.Collections.Generic;
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
                            if (enumerable == null)
                            {
                                return;
                            }

                            enumerable = !enumerable.Any() ? null : enumerable.ToList().OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();

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


        public static bool ClearBooks(string path)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<LocalBook>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        db.DeleteAll<LocalBook>();
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
