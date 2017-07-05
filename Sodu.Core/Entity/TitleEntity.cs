using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.Entity
{
   public class TitleEntity
    {
        public string TtitleId { get; set; }
        public string BookJosn { get; set; }

        /// <summary>
        /// 0 online 1 local
        /// </summary>
        public string BookType { get; set; }
    }
}
