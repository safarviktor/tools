using System;

namespace VClipboardHelper
{
    public static class Extensions
    {
        public static bool ContainsIgnoreCase(this string input, string expression)
        {
            return input.IndexOf(expression, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}