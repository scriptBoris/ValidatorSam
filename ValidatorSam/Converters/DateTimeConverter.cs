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
    /// default date time converter
    /// </summary>
    public class DateTimeConverter : DateTimeBaseConverter, IValueRawConverter<DateTime>
    {
        /// <param name="format">DateTime to string format</param>
        public DateTimeConverter(string? format)
        {
            InitStringFormat(format);
        }

        void IValueRawConverter<DateTime>.OnStringFormartChanged(string? newStringFormat)
        {
            InitStringFormat(newStringFormat);
        }

        /// <inheritdoc/>
        public ConverterResult<DateTime> RawToValue(ReadOnlySpan<char> rawValue, ReadOnlySpan<char> oldRawValue, DateTime oldValue, Validator validator)
        {
            var res = MasterParse(rawValue, validator);
            if (res.ResultType == ConverterResultType.Success)
            {
                return ConverterResult.Success<DateTime>((DateTime)res.Result!, res.RawResult!);
            }
            else
            {
                return ConverterResult.Error<DateTime>(res.ErrorText!, default, res.RawResult!);
            }
        }

        /// <inheritdoc/>
        public ConverterResult<DateTime> ValueToRaw([AllowNull] DateTime newValue, [AllowNull] DateTime oldValue, Validator validator)
        {
            if (validator.StringFormat != null)
            {
                string formatedRaw = newValue.ToString(validator.StringFormat);
                return ConverterResult.Success(newValue, formatedRaw);
            }
            else
            {
                string raw = newValue.ToString(DEFAULT_DATEFORMAT);
                return ConverterResult.Success(newValue, raw);
            }
        }
    }
#nullable disable
}
