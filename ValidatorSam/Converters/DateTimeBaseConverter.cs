using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ValidatorSam.Core;
using ValidatorSam.Internal;

namespace ValidatorSam.Converters
{
#nullable enable
    /// <summary>
    /// Basic functional for parsing text to datetime
    /// </summary>
    public abstract class DateTimeBaseConverter
    {
        /// <summary>
        /// Default StringFormat
        /// </summary>
        public const string DEFAULT_DATEFORMAT = "dd.MM.yyyy HH:mm";

        /// <summary>
        /// Global string format cache
        /// </summary>
        private static readonly ConcurrentDictionary<string, CacheItem> _cache = new ConcurrentDictionary<string, CacheItem>();

        /// <summary>
        /// Read and preparse StringFormat
        /// </summary>
        /// <exception cref="NotSupportedException">Not supported mask/format</exception>
        protected void InitStringFormat(string? format)
        {
        }

        /// <summary>
        /// Universal
        /// </summary>
        public static ConverterResult<DateTime?> MasterParse(ReadOnlySpan<char> rawValue, CultureInfo culture, string stringFormat)
        {
            switch (stringFormat)
            {
                case "d": // short date
                case "D": // long date
                case "g": // short date + short time
                case "G": // short date + long time
                case "m": // month day
                case "M": // month day
                case "y": // year month
                case "Y": // year month
                case "o": // ISON 8601
                case "O": // ISON 8601
                case "r": // RFC1123
                case "R": // RFC1123
                case "f": // full date + short time
                case "F": // full date + long time
                case "t": // short time
                case "T": // long time
                case "s": // sortable ISO-like
                case "u": // universal (sortable)
                case "U": // full date/time (UTC)
                    throw new NotSupportedException($"The StringFormat \"{stringFormat}\" is not supported");
                default:
                    break;
            }

            var bdate = new BundleDate();
            var sb = new StackStringBuilder50();

            var cacheItem = _cache.GetOrAdd(stringFormat, (x) =>
            {
                var list = new Dictionary<char, char>();
                var order = new Dictionary<DateTimeMembers, char>();
                foreach (var item in x)
                {
                    switch (item)
                    {
                        case 'y':
                        case 'Y':
                        case 'M':
                        case 'd':
                        case 'D':
                        case 'h':
                        case 'H':
                        case 'm':
                        case 's':
                            break;
                        default:
                            list.TryAdd(item, item);
                            break;
                    }

                    switch (item)
                    {
                        case 'y':
                        case 'Y':
                            order.TryAdd(DateTimeMembers.Year, item);
                            break;
                        case 'M':
                            order.TryAdd(DateTimeMembers.Month, item);
                            break;
                        case 'd':
                        case 'D':
                            order.TryAdd(DateTimeMembers.Day, item);
                            break;
                        case 'h':
                        case 'H':
                            order.TryAdd(DateTimeMembers.Hour, item);
                            break;
                        case 'm':
                            order.TryAdd(DateTimeMembers.Minute, item);
                            break;
                        case 's':
                            order.TryAdd(DateTimeMembers.Second, item);
                            break;
                        default:
                            break;
                    }
                }
                var output = new CacheItem
                {
                    AllowedChars = list.Select(x => x.Key).ToArray(),
                    FormatQueue = order.Select(x => x.Key).ToArray(),
                };
                return output;
            });
            foreach (var item in cacheItem.FormatQueue)
            {
                switch (item)
                {
                    case DateTimeMembers.Year:
                        bdate.year = -1;
                        break;
                    case DateTimeMembers.Month:
                        bdate.month = -1;
                        break;
                    case DateTimeMembers.Day:
                        bdate.day = -1;
                        break;
                    case DateTimeMembers.Hour:
                        bdate.hour = -1;
                        break;
                    case DateTimeMembers.Minute:
                        bdate.minute = -1;
                        break;
                    case DateTimeMembers.Second:
                        bdate.sec = -1;
                        break;
                    default:
                        break;
                }
            }

            ReadInputAndLittleParse(rawValue, stringFormat, cacheItem, ref sb, ref bdate);
            return ParseInt(culture, cacheItem, ref sb, ref bdate);
        }

        /// <summary>
        /// Post-parser
        /// </summary>
        internal static ConverterResult<DateTime?> ParseInt(CultureInfo culture, CacheItem cacheItem, ref StackStringBuilder50 rawStringBuilder, ref BundleDate bdate)
        {
            string raw = rawStringBuilder.ToString();
            int year = bdate.year;
            int month = bdate.month;
            int day = bdate.day;
            int hour = bdate.hour;
            int minute = bdate.minute;
            int sec = bdate.sec;

            bool badYear = (year <= 0 || year > 9999);
            bool badMonth = (month <= 0 || month > 12);
            bool badDay = (day <= 0);
            bool badHour = (hour < 0 || hour > 23);
            bool badMinute = (minute < 0 || minute > 59);
            bool badSec = (sec < 0 || sec > 59);

            int invalidMembers = bdate.InvalidMembersCount;
            int validMembers = 0;

            foreach (var item in cacheItem.FormatQueue)
            {
                switch (item)
                {
                    case DateTimeMembers.Year:
                        if (bdate.year > 0 && bdate.year <= 9999)
                            validMembers++;
                        break;
                    case DateTimeMembers.Month:
                        if (bdate.month > 0 && bdate.year <= 12)
                            validMembers++;
                        break;
                    case DateTimeMembers.Day:
                        if (bdate.day > 0 && bdate.day <= 31)
                            validMembers++;
                        break;
                    case DateTimeMembers.Hour:
                        if (bdate.hour >= 0 && bdate.hour <= 23)
                            validMembers++;
                        break;
                    case DateTimeMembers.Minute:
                        if (bdate.minute >= 0 && bdate.minute <= 59)
                            validMembers++;
                        break;
                    case DateTimeMembers.Second:
                        if (bdate.sec >= 0 && bdate.sec <= 59)
                            validMembers++;
                        break;
                    default:
                        break;
                }
            }

            if (invalidMembers > 0)
            {
                if (validMembers == 1 ||
                    invalidMembers == cacheItem.FormatQueue.Length)
                {
                    return ConverterResult.Error<DateTime?>(ValidatorLocalization.Resolve.StringInvalidInputForDateTime, null, raw);
                }
            }


            foreach (var item in cacheItem.FormatQueue)
            {
                switch (item)
                {
                    case DateTimeMembers.Year:
                        if (badYear)
                            return ConverterResult.Error<DateTime?>(ValidatorLocalization.Resolve.StringInvalidYear, null, raw);
                        break;
                    case DateTimeMembers.Month:
                        if (badMonth)
                            return ConverterResult.Error<DateTime?>(ValidatorLocalization.Resolve.StringInvalidMonth, null, raw);
                        break;
                    case DateTimeMembers.Day:
                        if (badDay)
                            return ConverterResult.Error<DateTime?>(ValidatorLocalization.Resolve.StringInvalidDay, null, raw);
                        break;
                    case DateTimeMembers.Hour:
                        if (badHour)
                            return ConverterResult.Error<DateTime?>(ValidatorLocalization.Resolve.StringInvalidHour, null, raw);
                        break;
                    case DateTimeMembers.Minute:
                        if (badMinute)
                            return ConverterResult.Error<DateTime?>(ValidatorLocalization.Resolve.StringInvalidMinute, null, raw);
                        break;
                    case DateTimeMembers.Second:
                        if (badSec)
                            return ConverterResult.Error<DateTime?>(ValidatorLocalization.Resolve.StringInvalidSecond, null, raw);
                        break;
                    default:
                        break;
                }
            }

            // try transfort 2 year to 4 year digits
            int finalYear;
            if (year >= 10 && year <= 99)
                finalYear = culture.Calendar.ToFourDigitYear(year);
            else
                finalYear = year;

            // check days in month
            int maxDays = DateTime.DaysInMonth(finalYear, month);
            if (day > maxDays)
            {
                string msg = string.Format(ValidatorLocalization.Resolve.StringMonthIsOverflow, maxDays);
                return ConverterResult.Error<DateTime?>(msg, null, raw);
            }

            try
            {
                return ConverterResult.Success<DateTime?>(new DateTime(finalYear, month, day, hour, minute, sec), raw);
            }
            catch (Exception)
            {
                return ConverterResult.Error<DateTime?>("Common date error", null, raw);
            }
        }

        /// <summary>
        /// Very rough parser
        /// </summary>
        internal static void ReadInputAndLittleParse(ReadOnlySpan<char> input, ReadOnlySpan<char> StringFormat,
            CacheItem chacheItem,
            ref StackStringBuilder50 outputRaw,
            ref BundleDate result)
        {
            var readyToParse = new ChainBuilder();
            int maskoffset = 0;
            char[] allowSpecialChars = chacheItem.AllowedChars;

            char inputChar;
            var maskCharType = MaskCharType.None;
            int endloop = input.Length;
            for (int i = 0; i < endloop; i++)
            {
                inputChar = input[i];

                char maskChar;
                if (i + maskoffset < StringFormat.Length)
                    maskChar = StringFormat[i + maskoffset];
                else
                    break; // TODO В будущем отладить когда пользователь вводит текст который длинее маски, но при этом содержит дату

                maskCharType = GetMaskCharType(maskChar);
                bool isMaskDigit = maskCharType != MaskCharType.None;
                bool isMaskSeparate = maskCharType == MaskCharType.None;
                bool isDigit = char.IsDigit(inputChar);

                // введенная цифра попала в маску (Y, M, D, etc...)
                if (isMaskDigit && isDigit)
                {
                    outputRaw.Append(inputChar);
                    readyToParse.PushNumberChain(inputChar, maskCharType, ref result);
                }
                // введенный символ попал в спецсимвол маски (lika a dots, comma, double dots and etc)
                else if (inputChar == maskChar)
                {
                    // на всякий случай проверяем, а не является ли введенный символ
                    // знаком маски (например "d")
                    if (GetMaskCharType(inputChar) == MaskCharType.None)
                    {
                        outputRaw.Append(inputChar);
                        readyToParse.FinalizingNumberChain(MaskCharType.None, ref result);
                    }
                }
                else if (readyToParse.Length > 0)
                {
                    // если юзер ввел цифру, вместо ожидаемой в маске спец символа
                    // то ставим ожидаемый спец символ(ы), а потом цифру
                    if (!isMaskDigit && isDigit)
                    {
                        outputRaw.Append(maskChar);
                        maskoffset++;

                        // скип
                        char nextMaskChar = default;
                        while (true)
                        {
                            int nextIndex = i + maskoffset;
                            if (nextIndex < StringFormat.Length)
                                nextMaskChar = StringFormat[nextIndex];
                            else
                                break;

                            if (GetMaskCharType(nextMaskChar) == MaskCharType.None)
                            {
                                outputRaw.Append(nextMaskChar);
                                maskoffset++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (nextMaskChar == default)
                        {
                            // выходим из for, т.к. введенная цифра находится вне маски
                            break;
                        }

                        var inputType = GetMaskCharType(nextMaskChar);
                        outputRaw.Append(inputChar);
                        readyToParse.PushNumberChain(inputChar, inputType, ref result);
                    }
                    // если юзер ввел спец символ, но маска ожидает цифру,
                    // то ставим спецсимвол и скипаем до следующей группы
                    else if (isMaskDigit && !isDigit)
                    {
                        // на всякий случай проверяем, а не является ли введенный символ
                        // знаком маски (например "d")
                        if (GetMaskCharType(inputChar) != MaskCharType.None)
                            continue;

                        // проверяем а действительно ли введен разрешенный спец символ?
                        if (!CheckAllowChar(inputChar, allowSpecialChars))
                            continue;

                        outputRaw.Append(inputChar);
                        maskoffset++;

                        // скип
                        char nextChar;
                        while (true)
                        {
                            int nextIndex = i + maskoffset;
                            if (nextIndex < StringFormat.Length)
                                nextChar = StringFormat[nextIndex];
                            else
                                break;

                            if (GetMaskCharType(nextChar) == maskCharType)
                                maskoffset++;
                            else
                                break;
                        }
                    }
                    else
                    {
                        readyToParse.FinalizingNumberChain(maskCharType, ref result);
                    }
                }
            }

            // если остались не расспаршенные символы в группе
            if (readyToParse.Length > 0)
            {
                readyToParse.FinalizingNumberChain(maskCharType, ref result);
            }
        }

        private static bool CheckAllowChar(char inputChar, char[] allowSpecialChars)
        {
            foreach (char c in allowSpecialChars)
            {
                if (c == inputChar)
                    return true;
            }
            return false;
        }

        private static MaskCharType GetMaskCharType(char c)
        {
            switch (c)
            {
                case 'd':
                case 'D':
                    return MaskCharType.Day;
                case 'M':
                    return MaskCharType.Month;
                case 'y':
                case 'Y':
                    return MaskCharType.Year;
                case 'm':
                    return MaskCharType.Minute;
                case 'h':
                case 'H':
                    return MaskCharType.Hour;
                case 's':
                case 'S':
                    return MaskCharType.Second;
                default:
                    return MaskCharType.None;
            }
        }

        internal enum MaskCharType
        {
            None = 0,
            Year,
            Month,
            Day,
            Minute,
            Hour,
            Second,
        }

        internal struct ChainBuilder
        {
            private StackStringBuilder50 _sb;
            private MaskCharType _current;

            public int Length => _sb.Length;
            public string DebugText => $"_____{_current}: <<{_sb.ToString()}>>_____";

            internal void PushNumberChain(char inputChar, MaskCharType mType, ref BundleDate result)
            {
                if (_current == mType)
                {
                    _sb.Append(inputChar);
                }
                else
                {
                    if (int.TryParse(_sb.AsSpan(), out int intParse))
                    {
                        switch (_current)
                        {
                            case MaskCharType.Year:
                                result.year = intParse;
                                break;
                            case MaskCharType.Month:
                                result.month = intParse;
                                break;
                            case MaskCharType.Day:
                                result.day = intParse;
                                break;
                            case MaskCharType.Hour:
                                result.hour = intParse;
                                break;
                            case MaskCharType.Minute:
                                result.minute = intParse;
                                break;
                            case MaskCharType.Second:
                                result.sec = intParse;
                                break;
                        }
                    }
                    _current = mType;
                    _sb.Clear();
                    _sb.Append(inputChar);
                }
            }

            internal void FinalizingNumberChain(MaskCharType mType, ref BundleDate result)
            {
                if (int.TryParse(_sb.AsSpan(), out int intParse))
                {
                    switch (_current)
                    {
                        case MaskCharType.Year:
                            result.year = intParse;
                            break;
                        case MaskCharType.Month:
                            result.month = intParse;
                            break;
                        case MaskCharType.Day:
                            result.day = intParse;
                            break;
                        case MaskCharType.Hour:
                            result.hour = intParse;
                            break;
                        case MaskCharType.Minute:
                            result.minute = intParse;
                            break;
                        case MaskCharType.Second:
                            result.sec = intParse;
                            break;
                    }
                }
                _sb.Clear();
            }
        }

        internal struct BundleDate
        {
            public int year;
            public int month;
            public int day;
            public int hour;
            public int minute;
            public int sec;

            public int InvalidMembersCount
            {
                get
                {
                    int m = 0;
                    if (year == -1) m++;
                    if (month == -1) m++;
                    if (day == -1) m++;
                    if (hour == -1) m++;
                    if (minute == -1) m++;
                    if (sec == -1) m++;
                    return m;
                }
            }
        }

        internal class CacheItem
        {
            public char[] AllowedChars { get; set; }
            public DateTimeMembers[] FormatQueue { get; set; }
        }

        internal enum DateTimeMembers
        {
            Day,
            Month,
            Year,
            Hour,
            Minute,
            Second,
        }
    }
#nullable disable
}
