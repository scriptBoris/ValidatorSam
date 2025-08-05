using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using ValidatorSam.Core;

namespace ValidatorSam.Converters
{
    #nullable enable
    /// <summary>
    /// Default converter for parsing user text input to number value
    /// </summary>
    public class NumbersConverter<T> : IValueRawConverter<T>
    {
        private readonly TypeCode _typeCode;

        /// <param name="code">TypeCode of generic type (T); if pass to null then it's will calculated automaticly</param>
        public NumbersConverter(TypeCode? code = null)
        {
            if (code != null)
            {
                _typeCode = code.Value;
            }
            else
            {
                var type = typeof(T);

                var nullableGenericType = Nullable.GetUnderlyingType(type);
                if (nullableGenericType != null)
                    type = nullableGenericType;

                _typeCode = Type.GetTypeCode(type);
            }
        }

        /// <inheritdoc/>
        public ConverterResult<T> RawToValue(ReadOnlySpan<char> rawValue, ReadOnlySpan<char> oldRaw, T oldValue, Validator validator)
        {
            switch (_typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return MasterParse(rawValue, oldRaw, oldValue, _typeCode, false, false);
                case TypeCode.Single:
                    return MasterParse(rawValue, oldRaw, oldValue, _typeCode, true, true, 7);
                case TypeCode.Double:
                    return MasterParse(rawValue, oldRaw, oldValue, _typeCode, true, true, 16);
                case TypeCode.Decimal:
                    return MasterParse(rawValue, oldRaw, oldValue, _typeCode, true, true, 28);
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return MasterParse(rawValue, oldRaw, oldValue, _typeCode, false, true);
                default:
                    return ConverterResult.Ignore<T>();
            }
        }

        /// <inheritdoc/>
        public ConverterResult<T> ValueToRaw([AllowNull] T newValue, [AllowNull] T oldValue, Validator validator)
        {
            if (newValue is IFormattable fnewValue && validator.StringFormat != null)
            {
                string raw = fnewValue.ToString(validator.StringFormat, validator.CultureInfo);
                return ConverterResult.Success<T>(newValue, raw);
            }
            else
            {
                string raw = newValue?.ToString() ?? "";
                return ConverterResult.Success<T>(newValue, raw);
            }
        }

        internal static ConverterResult<T> MasterParse(
            ReadOnlySpan<char> rawValue,
            ReadOnlySpan<char> oldRaw,
            T oldValue,
            TypeCode code,
            bool maybeComma,
            bool maybeNegative,
            int maxRawLength = -1)
        {
            var sbLogic = new StringBuilder(rawValue.Length);
            var sbRaw = new StringBuilder(rawValue.Length);

            if (maybeComma)
            {
                ProcessFloat(rawValue, sbLogic, sbRaw, maxRawLength);
            }
            else
            {
                if (maybeNegative)
                    ProcessInt(rawValue, sbLogic, sbRaw);
                else
                    ProcessIntPositive(rawValue, sbLogic, sbRaw);
            }

            string logic = sbLogic.ToString();
            string raw = sbRaw.ToString();
            bool hasMinus = false;

            if (maybeNegative)
            {
                hasMinus = logic.Length > 0 && logic[0] == '-';
            }

            T res;
            if (logic == "")
            {
                res = default;
            }
            else if (TryParse(logic, code, hasMinus, out var parseRes, out string? forceRaw))
            {
                res = parseRes;

                if (forceRaw != null)
                    raw = forceRaw;
            }
            else
            {
                res = oldValue;
            }

            return ConverterResult.Success<T>(res, raw);
        }

        // Awful method, but it is allow to avoid box-unbox operations and relieve GC load
        private static bool TryParse(string text, TypeCode codeType, bool hasMinus, out T result, out string? forceRaw)
        {
            var style = NumberStyles.Float;
            var culture = CultureInfo.InvariantCulture;
            result = default!;
            forceRaw = null;

            switch (codeType)
            {
                case TypeCode.Byte:
                    if (!byte.TryParse(text, out var parseByte))
                    {
                        parseByte = byte.MaxValue;
                        forceRaw = parseByte.ToString();
                    }

                    if (parseByte is T tbyte)
                        result = tbyte;

                    break;
                case TypeCode.UInt16:
                    if (!ushort.TryParse(text, out var parseUint16))
                    {
                        parseUint16 = UInt16.MaxValue;
                        forceRaw = parseUint16.ToString();
                    }

                    if (parseUint16 is T tUint16)
                        result = tUint16;

                    break;
                case TypeCode.UInt32:
                    if (!uint.TryParse(text, out var parseUint32))
                    {
                        parseUint32 = UInt32.MaxValue;
                        forceRaw = parseUint32.ToString();
                    }

                    if (parseUint32 is T tUint32)
                        result = tUint32;

                    break;
                case TypeCode.UInt64:
                    if (!ulong.TryParse(text, out var parseUint64))
                    {
                        parseUint64 = UInt64.MaxValue;
                        forceRaw = parseUint64.ToString();
                    }

                    if (parseUint64 is T tUint64)
                        result = tUint64;

                    break;
                case TypeCode.SByte:
                    if (!sbyte.TryParse(text, out var parseSbyte))
                    {
                        if (hasMinus)
                            parseSbyte = sbyte.MinValue;
                        else
                            parseSbyte = sbyte.MaxValue;
                        forceRaw = parseSbyte.ToString();
                    }

                    if (parseSbyte is T tSbyte)
                        result = tSbyte;

                    break;
                case TypeCode.Int16:
                    if (!short.TryParse(text, out var parseInt16))
                    {
                        if (hasMinus)
                            parseInt16 = Int16.MinValue;
                        else
                            parseInt16 = Int16.MaxValue;
                        forceRaw = parseInt16.ToString();
                    }

                    if (parseInt16 is T tInt16)
                        result = tInt16;

                    break;
                case TypeCode.Int32:
                    if (!int.TryParse(text, out var parseInt32))
                    {
                        if (hasMinus)
                            parseInt32 = Int32.MinValue;
                        else
                            parseInt32 = Int32.MaxValue;
                        forceRaw = parseInt32.ToString();
                    }

                    if (parseInt32 is T tInt32)
                        result = tInt32;

                    break;
                case TypeCode.Int64:
                    if (!long.TryParse(text, out var parseInt64))
                    {
                        if (hasMinus)
                            parseInt64 = Int64.MinValue;
                        else
                            parseInt64 = Int64.MaxValue;
                        forceRaw = parseInt64.ToString();
                    }

                    if (parseInt64 is T tInt64)
                        result = tInt64;

                    break;
                case TypeCode.Single:
                    if (float.TryParse(text, style, culture, out var parseFloat) && parseFloat is T tFloat)
                    {
                        result = tFloat;
                        return true;
                    }
                    break;
                case TypeCode.Double:
                    if (double.TryParse(text, style, culture, out var parseDouble) && parseDouble is T tDouble)
                    {
                        result = tDouble;
                        return true;
                    }
                    break;
                case TypeCode.Decimal:
                    if (decimal.TryParse(text, style, culture, out var parseDecimal) && parseDecimal is T tDecimal)
                    {
                        result = tDecimal;
                        return true;
                    }
                    break;
                default:
                    result = default!;
                    return false;
            }

            return true;
        }

        private static void ProcessInt(ReadOnlySpan<char> newValueStr, StringBuilder sbLogic, StringBuilder sbRaw)
        {
            bool hasZero = false;
            bool goNumbers = false;
            for (int i = 0; i < newValueStr.Length; i++)
            {
                char c = newValueStr[i];
                switch (c)
                {
                    case '.':
                    case ',':
                        i = newValueStr.Length;
                        break;
                    case '-':
                        if (sbLogic.Length == 0)
                        {
                            sbLogic.Append('-');
                            sbRaw.Append('-');
                        }
                        break;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (hasZero)
                        {
                            sbRaw.Remove(sbRaw.Length - 1, 1);
                            sbLogic.Remove(sbLogic.Length - 1, 1);
                        }

                        sbRaw.Append(c);
                        sbLogic.Append(c);

                        goNumbers = true;
                        hasZero = false;
                        break;
                    case '0':
                        if (goNumbers)
                        {
                            sbRaw.Append(c);
                            sbLogic.Append(c);
                        }
                        else
                        {
                            if (!hasZero)
                            {
                                sbRaw.Append('0');
                                sbLogic.Append('0');
                            }
                            hasZero = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (sbLogic.Length == 1)
            {
                if (sbLogic[0] == '-')
                {
                    sbLogic.Clear();
                    sbLogic.Append('0');
                }
            }
        }

        private static void ProcessIntPositive(ReadOnlySpan<char> newValueStr, StringBuilder sbLogic, StringBuilder sbRaw)
        {
            bool hasMinus = false;
            bool hasZero = false;
            bool goNumbers = false;
            for (int i = 0; i < newValueStr.Length; i++)
            {
                char c = newValueStr[i];
                switch (c)
                {
                    case '.':
                    case ',':
                        i = newValueStr.Length;
                        break;
                    case '-':
                        if (sbLogic.Length == 0)
                            hasMinus = true;
                        break;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (hasZero)
                        {
                            sbRaw.Remove(sbRaw.Length - 1, 1);
                            sbLogic.Remove(sbLogic.Length - 1, 1);
                        }

                        sbRaw.Append(c);
                        sbLogic.Append(c);

                        goNumbers = true;
                        hasZero = false;
                        break;
                    case '0':
                        if (goNumbers)
                        {
                            sbRaw.Append(c);
                            sbLogic.Append(c);
                        }
                        else
                        {
                            if (!hasZero)
                            {
                                sbRaw.Append('0');
                                sbLogic.Append('0');
                            }
                            hasZero = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (hasMinus)
            {
                sbLogic.Clear();
                sbRaw.Clear();
            }
        }

        private static void ProcessFloat(ReadOnlySpan<char> newValueStr, StringBuilder sbLogic, StringBuilder sbRaw, int limit)
        {
            bool hasMinus = false;
            bool hasZero = false;
            bool hasComma = false;
            bool goNumbers = false;
            for (int i = 0; i < newValueStr.Length; i++)
            {
                int numCount = sbLogic.Length;

                if (hasMinus)
                    numCount--;

                if (hasComma)
                    numCount--;

                if (numCount >= limit)
                    break;

                char c = newValueStr[i];
                switch (c)
                {
                    case '.':
                    case ',':
                        if (!hasComma)
                        {
                            hasComma = true;
                            goNumbers = true;
                            hasZero = false;
                            sbRaw.Append(c);
                            sbLogic.Append('.');
                        }
                        break;
                    case '-':
                        if (sbLogic.Length == 0)
                        {
                            sbLogic.Append('-');
                            sbRaw.Append('-');
                            hasMinus = true;
                        }
                        break;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (hasZero)
                        {
                            sbRaw.Remove(sbRaw.Length - 1, 1);
                            sbLogic.Remove(sbLogic.Length - 1, 1);
                        }

                        sbRaw.Append(c);
                        sbLogic.Append(c);

                        goNumbers = true;
                        hasZero = false;
                        break;
                    case '0':
                        if (goNumbers)
                        {
                            sbRaw.Append(c);
                            sbLogic.Append(c);
                        }
                        else
                        {
                            if (!hasZero)
                            {
                                sbRaw.Append('0');
                                sbLogic.Append('0');
                            }
                            hasZero = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (sbLogic.Length == 1)
            {
                if (sbLogic[0] == '-' || sbLogic[0] == '.')
                {
                    sbLogic.Clear();
                    sbLogic.Append('0');
                }
            }
        }
    }
    #nullable disable
}