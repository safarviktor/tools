using System;

namespace SqlTools
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        public static bool EqualsIgnoreCase(this string s, string anoher)
        {
            return string.Equals(s, anoher, StringComparison.InvariantCultureIgnoreCase);
        }
        
        public static string TrimIfNotNull(this string s)
        {
            return s.IsNullOrEmpty() ? s : s.Trim();
        }
    }
}