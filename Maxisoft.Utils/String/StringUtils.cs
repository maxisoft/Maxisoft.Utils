using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maxisoft.Utils.String
{
    public static class StringUtils
    {
        public static string FormatFromDictionary(this string formatString, Dictionary<string, object> valueDict)
        {
            var i = 0;
            var newFormatString = new StringBuilder(formatString);
            var keyToInt = new Dictionary<string, int>();
            foreach (var tuple in valueDict)
            {
                newFormatString = newFormatString.Replace("{" + tuple.Key + "}", "{" + i + "}");
                keyToInt.Add(tuple.Key, i);
                i++;
            }
            return string.Format(newFormatString.ToString(),
                valueDict.OrderBy(x => keyToInt[x.Key]).Select(x => x.Value).ToArray());
        }
    }
}