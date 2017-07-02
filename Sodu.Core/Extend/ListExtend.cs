using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.Extend
{
    public static class ListExtend
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> items, int numOfParts)
        {
            int i = 0;
            return items.GroupBy(x => i++ % numOfParts);
        }

    }
}
