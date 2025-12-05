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
        public ConverterResult<DateTime?> RawToValue(ReadOnlySpan<char> rawValue, ReadOnlySpan<char> oldRawValue, DateTime? oldValue, IValidator validator)
        {
            var culture = validator.CultureInfo ?? CultureInfo.CurrentCulture;
            var stringFormat = validator.StringFormat ?? DEFAULT_DATEFORMAT;
            var res = MasterParse(rawValue, culture, stringFormat);
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
        public string ValueToRaw(DateTime? newValue, IValidator validator)
        {
            if (newValue == null)
                return "";

            if (validator.StringFormat != null)
            {
                string formatedRaw = newValue.Value.ToString(validator.StringFormat);
                return formatedRaw;
            }
            else
            {
                string raw = newValue.Value.ToString(DEFAULT_DATEFORMAT);
                return raw;
            }
        }
    }
#nullable disable
}
