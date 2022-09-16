using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.Utils
{
    public static class CollectionExtMethod
    {
        public static TItem FirstOrDefault<TItem>(this IEnumerable<TItem> items, Predicate<TItem> predicate)
        {
            foreach(var item in items)
            {
                if (predicate(item)) return item;
            }
            return default(TItem);
        }
        public static object FirstOrDefault(this IEnumerable items, Predicate<object> predicate)
        {
            foreach(var item in items)
            {
                if (predicate(item)) return item;
            }
            return null;
        }
    }
}
