using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace Sodu.Core.Entity
{
    public class BookHistorySchema
    {
        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id { get; set; }
        public string BookId { get; set; }
        public string BookJson { get; set; }
        public string UpdateTime { get; set; }
    }
}
