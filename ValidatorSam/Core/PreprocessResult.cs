using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam.Core
{
    /// <summary>
    /// PreprocessResultType
    /// </summary>
    public enum PreprocessResultType
    {
        /// <summary>
        /// The preprocessor successful changed the value
        /// </summary>
        Success,

        /// <summary>
        /// An error occurred during preprocessor operation and the validator setted IsValid as false
        /// </summary>
        Error,

        /// <summary>
        /// the value will not modificated
        /// </summary>
        Ignore,
    }

    /// <summary>
    /// PreprocessResult
    /// </summary>
    public class PreprocessResult
    {
        private PreprocessResult()
        {
        }

        /// <summary>
        /// Preprocessor result
        /// </summary>
        public PreprocessResultType ResultType { get; private set; }

        /// <summary>
        /// Error where will displayed by the Validator
        /// </summary>
        public string? ErrorText { get; private set; }

        /// <summary>
        /// Modificated Value
        /// </summary>
        public object? ValueResult { get; private set; }

        /// <summary>
        /// Modificated RawValue
        /// </summary>
        public string? RawResult { get; private set; }

        /// <summary>
        /// The preprocessor successful changed the value
        /// </summary>
        /// <param name="value">modificated Value</param>
        /// <param name="raw">modificated RawValue</param>
        public static PreprocessResult Success(object? value, string? raw)
        {
            return new PreprocessResult
            {
                ResultType = PreprocessResultType.Success,
                ValueResult = value,
                RawResult = raw,
            };
        }

        /// <summary>
        /// An error occurred during preprocessor operation
        /// </summary>
        /// <param name="errorText">Required error text</param>
        /// <param name="value">modificated Value</param>
        /// <param name="raw">modificated RawValue</param>
        public static PreprocessResult Error(string errorText, object? value, string? raw)
        {
            return new PreprocessResult
            {
                ResultType = PreprocessResultType.Error,
                ErrorText = errorText,
                ValueResult = value,
                RawResult = raw,
            };
        }

        /// <summary>
        /// the value will not modificated
        /// </summary>
        public static PreprocessResult Ignore()
        {
            return new PreprocessResult() { ResultType = PreprocessResultType.Ignore };
        }
    }
}
#nullable disable