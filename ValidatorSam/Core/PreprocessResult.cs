using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class PreprocessResult<T>
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
        [AllowNull]
        public T ValueResult { get; private set; }

        /// <summary>
        /// The preprocessor successful changed the value
        /// </summary>
        /// <param name="value">modificated Value</param>
        public static PreprocessResult<T> Success([AllowNull]T value)
        {
            return new PreprocessResult<T>
            {
                ResultType = PreprocessResultType.Success,
                ValueResult = value,
            };
        }

        /// <summary>
        /// An error occurred during preprocessor operation
        /// </summary>
        /// <param name="errorText">Required error text</param>
        /// <param name="value">modificated Value</param>
        public static PreprocessResult<T> Error(string errorText, T value)
        {
            return new PreprocessResult<T>
            {
                ResultType = PreprocessResultType.Error,
                ErrorText = errorText,
                ValueResult = value,
            };
        }

        /// <summary>
        /// the value will not modificated
        /// </summary>
        public static PreprocessResult<T> Ignore()
        {
            return new PreprocessResult<T>() { ResultType = PreprocessResultType.Ignore };
        }
    }
}
#nullable disable