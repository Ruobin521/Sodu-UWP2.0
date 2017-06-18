using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace Sodu.Core.Entity
{
    public class BookCatalog
    {

        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id { get; set; }
        public string BookId { get; set; }
        public string CatalogName { get; set; }
        public string CatalogUrl { get; set; }
        public string LyWeb { get; set; }
        public int Index { get; set; }

    }

    public class BookCatalogContent
    {

        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id { get; set; }
        public string BookId { get; set; }
        public string Content { get; set; }
        public string CatalogUrl { get; set; }
    }
}
