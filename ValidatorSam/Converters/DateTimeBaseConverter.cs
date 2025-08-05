using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ValidatorSam.Core;

namespace ValidatorSam.Converters
{
#nullable enable
    /// <summary>
    /// Basic functional for parsing text to datetime
    /// </summary>
    public abstract class DateTimeBaseConverter
    {
        private Dictionary<int, char> _allowSpecialChars = new Dictionary<int, char>();

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
            switch (format)
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
                    throw new NotSupportedException($"The StringFormat \"{format}\" is not supported");
                default:
                    break;
            }

            _allowSpecialChars.Clear();

            string f = format ?? DEFAULT_DATEFORMAT;
            for (int i = 0; i < f.Length; i++)
            {
                char c = f[i];
                switch (c)
                {
                    case 'd':
                    case 'D':
                    case 'm':
                    case 'M':
                    case 'y':
                    case 'Y':
                    case 'h':
                    case 'H':
                    case 's':
                    case 'S':
                        continue;
                    default:
                        break;
                }

                _allowSpecialChars.TryAdd(c, c);
            }
        }

        /// <summary>
        /// Universal
        /// </summary>
        protected ConverterResult<DateTime?> MasterParse(ReadOnlySpan<char> rawValue, Validator validator)
        {
            var culture = validator._cultureInfo ?? CultureInfo.CurrentCulture;
            var stringFormat = (validator.StringFormat ?? DEFAULT_DATEFORMAT).AsSpan();
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int sec = 0;

            foreach (var item in stringFormat)
            {
                switch (item)
                {
                    case 'y':
                    case 'Y':
                        year = -1;
                        break;
                    case 'M':
                        month = -1;
                        break;
                    case 'd':
                    case 'D':
                        day = -1;
                        break;
                    case 'h':
                    case 'H':
                        hour = -1;
                        break;
                    case 'm':
                        minute = -1;
                        break;
                    case 's':
                        sec = -1;
                        break;
                    default:
                        break;
                }
            }

            var sb = new StringBuilder(rawValue.Length);
            ReadInputAndLittleParse(rawValue, stringFormat, sb, ref year, ref month, ref day, ref hour, ref minute, ref sec);
            return ParseInt(year, month, day, hour, minute, sec, sb, culture);
        }

        /// <summary>
        /// Post-parser
        /// </summary>
        protected ConverterResult<DateTime?> ParseInt(int year, int month, int day, int hour, int minute, int sec, StringBuilder sb, CultureInfo culture)
        {
            string raw = sb.ToString();

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
        protected void ReadInputAndLittleParse(ReadOnlySpan<char> input, ReadOnlySpan<char> StringFormat, StringBuilder output,
            ref int year, ref int month, ref int day,
            ref int hour, ref int minute, ref int sec)
        {
            var readyToParse = new StringBuilder();
            var readyToParseType = MaskCharType.None;
            int maskoffset = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char inputChar = input[i];
                char maskChar;

                if (i + maskoffset < StringFormat.Length)
                    maskChar = StringFormat[i + maskoffset];
                else
                    maskChar = default;

                var mType = CheckIsMaskChar(maskChar);
                bool isMask = mType != MaskCharType.None;
                bool isDigit = char.IsDigit(inputChar);
                var inputType = MaskCharType.None;

                // введенная цифра попала в маску (Y, M, D, etc...)
                if (isMask && isDigit)
                {
                    output.Append(inputChar);
                    inputType = mType;
                }
                // введенный символ попал в спецсимвол маски (lika a dots, comma, double dots and etc)
                else if (inputChar == maskChar)
                {
                    // на всякий случай проверяем, а не является ли введенный символ
                    // знаком маски (например "d")
                    if (CheckIsMaskChar(inputChar) == MaskCharType.None)
                        output.Append(inputChar);
                }
                else if (readyToParse.Length > 0)
                {
                    // если юзер ввел спец символ, но маска ожидает цифру,
                    // то ставим спецсимвол и скипаем до следующей группы
                    if (isMask && !isDigit && SpecialCharMask(inputChar))
                    {
                        output.Append(inputChar);
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

                            if (CheckIsMaskChar(nextChar) == mType)
                                maskoffset++;
                            else
                                break;
                        }
                    }
                    // если юзер ввел цифру, вместо ожидаемой в маске спец символа
                    // то ставим ожидаемый спец символ(ы), а потом цифру
                    else if (!isMask && isDigit)
                    {
                        output.Append(maskChar);
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

                            if (CheckIsMaskChar(nextMaskChar) == MaskCharType.None)
                            {
                                output.Append(nextMaskChar);
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

                        inputType = CheckIsMaskChar(nextMaskChar);
                        output.Append(inputChar);
                    }
                }

                if (inputType != MaskCharType.None)
                {
                    if (readyToParseType == inputType || readyToParseType == MaskCharType.None)
                    {
                        readyToParse.Append(inputChar);
                        readyToParseType = inputType;
                    }
                    else
                    {
                        string value = readyToParse.ToString();
                        if (int.TryParse(value, out int parseValue))
                        {
                            switch (readyToParseType)
                            {
                                case MaskCharType.Year:
                                    year = parseValue;
                                    break;
                                case MaskCharType.Month:
                                    month = parseValue;
                                    break;
                                case MaskCharType.Day:
                                    day = parseValue;
                                    break;
                                case MaskCharType.Hour:
                                    hour = parseValue;
                                    break;
                                case MaskCharType.Minute:
                                    minute = parseValue;
                                    break;
                                case MaskCharType.Second:
                                    sec = parseValue;
                                    break;
                            }
                        }
                        readyToParse.Clear();
                        readyToParse.Append(inputChar);
                        readyToParseType = inputType;
                    }
                }
            }

            // если остались не расспаршенные символы
            if (readyToParse.Length > 0)
            {
                string value = readyToParse.ToString();
                if (int.TryParse(value, out int parseValue))
                {
                    switch (readyToParseType)
                    {
                        case MaskCharType.Year:
                            year = parseValue;
                            break;
                        case MaskCharType.Month:
                            month = parseValue;
                            break;
                        case MaskCharType.Day:
                            day = parseValue;
                            break;
                        case MaskCharType.Hour:
                            hour = parseValue;
                            break;
                        case MaskCharType.Minute:
                            minute = parseValue;
                            break;
                        case MaskCharType.Second:
                            sec = parseValue;
                            break;
                    }
                }
            }
        }

        private MaskCharType CheckIsMaskChar(char c)
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

        private bool SpecialCharMask(char c)
        {
            return _allowSpecialChars.ContainsValue(c);
        }

        private enum MaskCharType
        {
            None = 0,
            Year,
            Month,
            Day,
            Minute,
            Hour,
            Second,
        }
    }
#nullable disable
}
