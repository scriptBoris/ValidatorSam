using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using ValidatorSam.Core;

namespace ValidatorSam.Converters
{
#nullable enable
    /// <inheritdoc cref="DateTimeConverter"/>
    public class DateTimeNullConverter : DateTimeBaseConverter, IValueRawConverter<DateTime?>
    {
        /// <param name="format">DateTime to string format</param>
        public DateTimeNullConverter(string? format)
        {
            InitStringFormat(format);
        }

        void IValueRawConverter<DateTime?>.OnStringFormartChanged(string? newStringFormat)
        {
            InitStringFormat(newStringFormat);
        }

        /// <inheritdoc/>
        public ConverterResult<DateTime?> RawToValue(ReadOnlySpan<char> rawValue, ReadOnlySpan<char> oldRawValue, DateTime? oldValue, Validator validator)
        {
            var res = MasterParse(rawValue, validator);
            if (res.ResultType == ConverterResultType.Success)
            {
                return ConverterResult.Success<DateTime?>(res.Result!, res.RawResult!);
            }
            else
            {
                return ConverterResult.Error<DateTime?>(res.ErrorText!, default, res.RawResult!);
            }
        }

        /// <inheritdoc/>
        public ConverterResult<DateTime?> ValueToRaw([AllowNull] DateTime? newValue, [AllowNull] DateTime? oldValue, Validator validator)
        {
            if (newValue == null)
                return ConverterResult.Success(newValue, "");

            if (validator.StringFormat != null)
            {
                string formatedRaw = newValue.Value.ToString(validator.StringFormat);
                return ConverterResult.Success(newValue, formatedRaw);
            }
            else
            {
                string raw = newValue.Value.ToString(DEFAULT_DATEFORMAT);
                return ConverterResult.Success(newValue, raw);
            }
        }
    }
#nullable disable
}
