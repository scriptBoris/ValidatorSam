using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorTests.Supports
{
    public static class StringExt
    {
        public static string NormalizeSpaces(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input
                .Replace('\u00A0', ' ')   // неразрывный пробел → обычный
                .Replace('\u2007', ' ')   // фигурный пробел
                .Replace('\u202F', ' ');  // узкий неразрывный
        }
    }
}