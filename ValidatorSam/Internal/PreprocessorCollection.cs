using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidatorSam.Core;
using static System.Net.Mime.MediaTypeNames;

#nullable enable
namespace ValidatorSam.Internal
{
    internal static class PreprocessorCollection
    {
        private struct NumbericTryParse<T> where T : struct
        {
            public T ParseResult { get; private set; }

            public bool TryParse(string text)
            {
                object? res = null;
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Byte:
                        if (byte.TryParse(text, out byte byteRes))
                            res = byteRes;
                        break;
                    case TypeCode.SByte:
                        if (sbyte.TryParse(text, out sbyte sbyteRes))
                            res = sbyteRes;
                        break;
                    case TypeCode.UInt16:
                        if (ushort.TryParse(text, out ushort ushortRes))
                            res = ushortRes;
                        break;
                    case TypeCode.UInt32:
                        if (uint.TryParse(text, out uint uintRes))
                            res = uintRes;
                        break;
                    case TypeCode.UInt64:
                        if (ulong.TryParse(text, out ulong ulongRes))
                            res = ulongRes;
                        break;
                    case TypeCode.Int16:
                        if (short.TryParse(text, out short shortRes))
                            res = shortRes;
                        break;
                    case TypeCode.Int32:
                        if (int.TryParse(text, out int intRes))
                            res = intRes;
                        break;
                    case TypeCode.Int64:
                        if (long.TryParse(text, out long longRes))
                            res = longRes;
                        break;
                    case TypeCode.Decimal:
                        if (decimal.TryParse(text, out decimal decimalRes))
                            res = decimalRes;
                        break;
                    case TypeCode.Double:
                        if (double.TryParse(text, out double doubleRes))
                            res = doubleRes;
                        break;
                    case TypeCode.Single:
                        if (float.TryParse(text, out float floatRes))
                            res = floatRes;
                        break;
                    default:
                        break;
                }

                if (res is T t)
                    ParseResult = t;

                return res != null;
            }
        }

        internal static PreprocessResult CastInt32(ValidatorPreprocessArgs x)
        {
            return MasterParse<int>(x, false, true);
        }

        internal static PreprocessResult CastDouble(ValidatorPreprocessArgs x)
        {
            return MasterParse<double>(x, true, true);
        }

        internal static PreprocessResult MasterParse<T>(ValidatorPreprocessArgs args, bool maybeComma, bool maybeNegative)
            where T : struct
        {
            if (!args.IsString)
                return new PreprocessResult { IsSkip = true };

            if (string.IsNullOrEmpty(args.StringNewValue))
                return new PreprocessResult { Result = null };

            var parser = new NumbericTryParse<T>();
            string logic = args.StringNewValue.Replace(" ", "");

            // fix different commas
            if (maybeComma)
                logic = logic.Replace(".", ",");

            // clear repeatable zeros
            logic = FixFirstZeros(logic, maybeComma, maybeNegative);

            string? text = logic;

            // fix multiple "-"
            if (maybeNegative)
                FixMultiMunis(ref logic, ref text);

            object? res;
            if (parser.TryParse(logic))
            {
                res = parser.ParseResult;
            }
            else
            {
                string? repl = null;
                res = args.OldValue;

                if (maybeNegative && text == "-")
                    repl = "-";

                text = repl ?? res?.ToString();
            }

            return new PreprocessResult
            {
                Result = res,
                TextResult = text,
            };
        }

        private static string FixFirstZeros(string text, bool maybeComma, bool maybeNegative)
        {
            int offset = -1;
            int count = 0;
            bool staySolo = true;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '0')
                {
                    if (offset < 0)
                        offset = i;
                    count++;
                }
                else if (offset >= 0)
                {
                    if (c == ',')
                        staySolo = true;
                    else
                        staySolo = false;

                    break;
                }
                else if (c != '-')
                {
                    break;
                }
            }

            if (count == 0)
                return text;

            string res;
            if (staySolo)
                res = text.Remove(offset, count - 1);
            else
                res = text.Remove(offset, count);

            return res;
        }

        private static void FixMultiMunis(ref string logic, ref string text)
        {
            if (logic.Length == 1 && logic.FirstOrDefault() == '-')
                logic = logic.Remove(0);
            else
            {
                int? countMinus = 0;
                foreach (char c in logic)
                    if (c == '-')
                        countMinus++;
                    else
                        break;

                countMinus--;
                if (countMinus < 0)
                    countMinus = null;

                if (countMinus != null)
                {
                    logic = logic.Remove(0, countMinus.Value);
                    text = text.Remove(0, countMinus.Value);
                }
            }
        }
    }
}
#nullable disable