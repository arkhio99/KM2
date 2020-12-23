using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KM2
{
    public static class IListExtensions
    {
        public static string StringView<T>(this IList<T> list, string separator = "; ")
        {
            StringBuilder sb = new StringBuilder("[");
            var str = string.Join(separator, (from val in list select val.ToString()).ToArray());
            return sb.Append(str).Append("]").ToString();
        }
    }
}
