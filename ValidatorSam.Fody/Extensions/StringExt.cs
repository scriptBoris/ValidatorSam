using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam.Fody.Extensions
{
    public static class StringExt
    {
        public static string Multiple(this string s, int multiple) 
        {
            string res = "";

            for (int i = 0; i < multiple; i++)
                res += s;

            return res;
        }

        public static string ExtractTextInsideAngleBrackets(this string input)
        {
            int startIndex = input.IndexOf("<");
            int endIndex = input.LastIndexOf(">");

            if (startIndex < 0 || endIndex < 0 || startIndex >= endIndex)
                return null;

            int nestedStartIndex = startIndex;
            int nestedEndIndex = startIndex;

            while (nestedEndIndex < endIndex)
            {
                nestedEndIndex = input.IndexOf(">", nestedEndIndex + 1);
                int nestedStartIndexNew = input.IndexOf("<", nestedStartIndex + 1);
                if (nestedEndIndex >= nestedStartIndexNew && nestedStartIndexNew != -1)
                {
                    nestedStartIndex = nestedStartIndexNew;
                }
            }

            return input.Substring(startIndex + 1, nestedEndIndex - startIndex - 1);
        }

    }
}
