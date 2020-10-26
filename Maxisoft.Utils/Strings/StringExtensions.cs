using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Maxisoft.Utils.Strings
{
    public static class StringExtensions
    {
        public static string FormatWithDictionary<TValue>(this string formatString,
            IDictionary<string, TValue> valueDict)
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
                    arr[i] = kv.Value!;
                    i++;
                }
            }

            return string.Format(f, arr);
        }

        public static string Capitalize(this string str)
        {
            if (str.Length <= 1)
            {
                return str.ToUpper();
            }

            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string Capitalize(this string str, CultureInfo cultureInfo)
        {
            if (str.Length <= 1)
            {
                return str.ToUpper(cultureInfo);
            }

            return char.ToUpper(str[0], cultureInfo) + str.Substring(1);
        }


        public static void Capitalize(this ReadOnlySpan<char> str, Span<char> destination)
        {
            if (str.Length == 0)
            {
                return;
            }

            destination[0] = char.ToUpper(str[0]);
            if (str.Length > 1)
            {
                str.Slice(1).CopyTo(destination.Slice(1));
            }
        }

        public static void Capitalize(this ReadOnlySpan<char> str, Span<char> destination, CultureInfo cultureInfo)
        {
            if (str.Length <= 1)
            {
                str.ToUpper(destination, cultureInfo);
                return;
            }

            destination[0] = char.ToUpper(str[0], cultureInfo);
            str.Slice(1).CopyTo(destination.Slice(1));
        }


        public static void Capitalize(this Span<char> str, CultureInfo cultureInfo)
        {
            if (str.Length == 0)
            {
                return;
            }

            str[0] = char.ToUpper(str[0], cultureInfo);
        }

        public static void Capitalize(this Span<char> str)
        {
            if (str.Length == 0)
            {
                return;
            }

            str[0] = char.ToUpper(str[0]);
        }


        public static string Repeat(this string str, int times)
        {
            if (times < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times), times, "negative isn't allowed");
            }

            var sb = new StringBuilder(checked(str.Length * times));
            for (var i = 0; i < times; i++)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }

        public static string Repeat(this char str, int times)
        {
            if (times < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times), times, "negative isn't allowed");
            }

            return new string(str, times);
        }

        public static void Repeat(this ReadOnlySpan<char> str, Span<char> destination, int times)
        {
            if (times < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times), times, "negative isn't allowed");
            }

            if (destination.Length < times * str.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (var i = 0; i < times; i++)
            {
                str.CopyTo(destination.Slice(checked(str.Length * i)));
            }
        }

        public static void Repeat(this char c, Span<char> destination, int times)
        {
            if (times < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times), times, "negative isn't allowed");
            }

            if (destination.Length < times)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (var i = 0; i < times; i++)
            {
                destination[i] = c;
            }
        }


        public static bool IsUpper(this string str)
        {
            return str.All(t => !char.IsUpper(t));
        }

        public static bool IsUpper(this ReadOnlySpan<char> str)
        {
            foreach (var t in str)
            {
                if (char.IsUpper(t))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsLower(this string value)
        {
            return value.All(t => !char.IsLower(t));
        }

        public static bool IsLower(this ReadOnlySpan<char> str)
        {
            foreach (var t in str)
            {
                if (char.IsLower(t))
                {
                    return false;
                }
            }

            return true;
        }
    }
}