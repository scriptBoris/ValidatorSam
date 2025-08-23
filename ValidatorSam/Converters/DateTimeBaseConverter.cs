using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
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
        /// Read and preparse StringFormat
        /// </summary>
        /// <exception cref="NotSupportedException">Not supported mask/format</exception>
        protected void InitStringFormat(string? format)
        {
        }

        /// <summary>
        /// Universal
        /// </summary>
        public ConverterResult<DateTime?> MasterParse(ReadOnlySpan<char> rawValue, CultureInfo culture, string stringFormat)
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

            foreach (var item in stringFormat)
            {
                switch (item)
                {
                    case 'y':
                    case 'Y':
                        bdate.year = -1;
                        break;
                    case 'M':
                        bdate.month = -1;
                        break;
                    case 'd':
                    case 'D':
                        bdate.day = -1;
                        break;
                    case 'h':
                    case 'H':
                        bdate.hour = -1;
                        break;
                    case 'm':
                        bdate.minute = -1;
                        break;
                    case 's':
                        bdate.sec = -1;
                        break;
                    default:
                        break;
                }
            }
            
            ReadInputAndLittleParse(rawValue, stringFormat, ref sb, ref bdate);
            return ParseInt(culture, ref sb, ref bdate);
        }

        /// <summary>
        /// Post-parser
        /// </summary>
        internal ConverterResult<DateTime?> ParseInt(CultureInfo culture, ref StackStringBuilder50 rawStringBuilder, ref BundleDate bdate)
        {
            string raw = rawStringBuilder.ToString();
            int year = bdate.year;
            int month = bdate.month;
            int day = bdate.day;
            int hour = bdate.hour;
            int minute = bdate.minute;
            int sec = bdate.sec;

            if (year <= 0 || year > 9999)
                return ConverterResult.Error<DateTime?>("Invalid date", null, raw);

            if (month <= 0 || month > 12)
                return ConverterResult.Error<DateTime?>("Invalid month", null, raw);

            if (day <= 0)
                return ConverterResult.Error<DateTime?>("Invalid day", null, raw);

            if (hour < 0 || hour > 23)
                return ConverterResult.Error<DateTime?>("Invalid time", null, raw);

            if (minute < 0 || minute > 59)
                return ConverterResult.Error<DateTime?>("Invalid time", null, raw);

            if (sec < 0 || sec > 59)
                return ConverterResult.Error<DateTime?>("Invalid time", null, raw);

            // try transfort 2 year to 4 year digits
            int finalYear;
            if (year >= 10 && year <= 99)
                finalYear = culture.Calendar.ToFourDigitYear(year);
            else
                finalYear = year;

            // check days in month
            int maxDays = DateTime.DaysInMonth(finalYear, month);
            if (day > maxDays)
                return ConverterResult.Error<DateTime?>($"Month no contains {day} days", null, raw);

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
        internal void ReadInputAndLittleParse(ReadOnlySpan<char> input, ReadOnlySpan<char> StringFormat,
            ref StackStringBuilder50 outputRaw,
            ref BundleDate result)
        {
            var readyToParse = new ChainBuilder();
            int maskoffset = 0;

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
                    maskChar = default;

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

        private MaskCharType GetMaskCharType(char c)
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
        }
    }
#nullable disable
}
