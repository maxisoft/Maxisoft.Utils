using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Maxisoft.Utils.Strings
{
    public static class StringExtensions
    {
        public static string FormatWithDictionary<TValue>(this string formatString, Dictionary<string, TValue> valueDict)
        {
            var i = 0;
            var f = formatString;
            var arr = new object[valueDict.Count];
            foreach (var kv in valueDict)
            {
                var needle = kv.Key;
                if (needle.Contains('"'))
                {
                    throw new ArgumentException("Forbidden character \" found in key");
                }

                if (needle.Trim('{', '}') != needle)
                {
                    throw new ArgumentException("Forbidden character { or } found in key head/tail");
                }
                needle = needle.Replace("{", "{{");
                needle = needle.Replace("}", "}}");
                var regex = @"({*)(?<!(""|')){\s*(" + Regex.Escape(needle) + @")\s*(:\s*\w+)??}(?!(""|'))(}*)";
                var matches = Regex.Matches(f, regex);
                var matchCounter = 0;
                foreach (Match match in matches)
                {
                    if (match.Groups[6].Length != match.Groups[1].Length)
                    {
                        throw new ArgumentException($"Unbalanced bracket for {kv.Key} #{matchCounter}");
                    }

                    if (match.Groups[1].Length % 2 != 0)
                    {
                        throw new ArgumentException($"Unbalanced bracket for {kv.Key} #{matchCounter}");
                    }

                    matchCounter += 1;
                }

                if (matchCounter > 0)
                {
                    var replacement = @"$1{" + i + @"$4}$6";
                    f = Regex.Replace(f, regex, replacement);
                    arr[i] = kv.Value;
                    i++;
                }
                
            }

            return string.Format(f, arr);
        }
    }
}