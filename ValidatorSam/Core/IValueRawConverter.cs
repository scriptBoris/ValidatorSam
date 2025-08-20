using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace ValidatorSam.Core
{
#nullable enable
    /// <summary>
    /// Converts user text input into a value of type <typeparamref name="T"/>
    /// <br/>
    /// Please, don't use this class for impelentation custom logic of validation
    /// </summary>
    /// <typeparam name="T">The target value type.</typeparam>
    public interface IValueRawConverter<T>
    {
        /// <summary>
        /// If Validator StringFormat has been changed at runtime, the converter will 
        /// know about the change in this method.
        /// </summary>
        virtual void OnStringFormartChanged(string? newStringFormat)
        {
        }

        /// <summary>
        /// If Validator CultureInfo has been changed at runtime, the converter will 
        /// know about the change in this method.
        /// </summary>
        virtual void OnCultureInfoChanged(CultureInfo? newCulture)
        {
        }

        /// <summary>
        /// Converts user text input (RawValue) into to value of type (Value)
        /// </summary>
        /// <param name="rawValue">Current user input text</param>
        /// <param name="oldRawValue">Previous user input text</param>
        /// <param name="oldValue">The old value that is already valid and parsed</param>
        /// <param name="validator">Invoker.</param>
        ConverterResult<T> RawToValue(ReadOnlySpan<char> rawValue, ReadOnlySpan<char> oldRawValue, T oldValue, Validator validator);

        /// <summary>
        /// Converts value of type (Value) into user text input (RawValue)
        /// </summary>
        /// <param name="newValue">New value</param>
        /// <param name="validator">Invoker.</param>
        ConverterResult<T> ValueToRaw(T newValue, Validator validator);
    }

    /// <summary>
    /// ConverterResultTypes
    /// </summary>
    public enum ConverterResultType
    {
        /// <summary>
        /// The converter successful operation
        /// </summary>
        Success,

        /// <summary>
        /// An error occurred during convert operation and the validator setted IsValid as false
        /// </summary>
        Error,

        /// <summary>
        /// Convert operation will skiped
        /// </summary>
        Skip,
    }

    /// <summary>
    /// Container of conversion result
    /// </summary>
    public struct ConverterResult<T>
    {
        /// <summary>
        /// Preprocessor result
        /// </summary>
        public ConverterResultType ResultType { get; internal set; }

        /// <summary>
        /// Error where will displayed by the Validator
        /// </summary>
        public string? ErrorText { get; internal set; }

        /// <summary>
        /// Value
        /// </summary>
        [AllowNull]
        public T Result { get; internal set; }

        /// <summary>
        /// Raw value
        /// </summary>
        public string? RawResult { get; internal set; }
    }

    /// <summary>
    /// Static class for making instance for ConverterResult[T]
    /// </summary>
    public struct ConverterResult
    {
        /// <summary>
        /// The conveter successful changed the value
        /// </summary>
        /// <param name="value">modificated Value</param>
        /// <param name="rawValue">modificated text value (RawValue)</param>
        public static ConverterResult<T2> Success<T2>([AllowNull]T2 value, string rawValue)
        {
            return new ConverterResult<T2>
            {
                ResultType = ConverterResultType.Success,
                Result = value,
                RawResult = rawValue,
            };
        }

        /// <summary>
        /// The conveter successful changed the value
        /// </summary>
        /// <param name="value">modificated Value</param>
        /// <param name="rawValue">modificated text value (RawValue)</param>
        public static ConverterResult<T2> Success<T2>(T2 value, ReadOnlySpan<char> rawValue)
        {
            return new ConverterResult<T2>
            {
                ResultType = ConverterResultType.Success,
                Result = value,
                RawResult = rawValue.ToString(),
            };
        }

        /// <summary>
        /// An error occurred during conveterer operation
        /// </summary>
        /// <param name="errorText">required error text</param>
        /// <param name="value">modificated Value</param>
        /// <param name="rawValue">modificated RawValue</param>
        public static ConverterResult<T> Error<T>(string errorText, T value, string rawValue)
        {
            return new ConverterResult<T>
            {
                ResultType = ConverterResultType.Error,
                ErrorText = errorText,
                Result = value,
                RawResult = rawValue,
            };
        }

        /// <summary>
        /// An error occurred during conveterer operation
        /// </summary>
        /// <param name="errorText">required error text</param>
        /// <param name="value">modificated Value</param>
        /// <param name="rawValue">modificated RawValue</param>
        public static ConverterResult<T> Error<T>(string errorText, [AllowNull] T value, ReadOnlySpan<char> rawValue)
        {
            return new ConverterResult<T>
            {
                ResultType = ConverterResultType.Error,
                ErrorText = errorText,
                Result = value,
                RawResult = rawValue.ToString(),
            };
        }

        /// <summary>
        /// Convert operation will skiped
        /// </summary>
        public static ConverterResult<T> Ignore<T>()
        {
            return new ConverterResult<T>() { ResultType = ConverterResultType.Skip };
        }
    }
#nullable disable
}
