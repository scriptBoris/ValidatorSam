using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ValidatorSam.Core;

namespace ValidatorSam.Internal
{
#nullable enable
    internal readonly struct HandleRawResult<T>
    {
        internal HandleRawResult(
            ValidatorResult? prepError,
            [AllowNull] T newValue,
            PreprocessResultType type)
        {
            PrepError = prepError;
            NewValue = newValue;
            ResultType = type;
        }

        internal PreprocessResultType ResultType { get; }

        /// <summary>
        /// Ошибка препроцессора
        /// </summary>
        internal ValidatorResult? PrepError { get; }

        /// <summary>
        /// Значение
        /// </summary>
        [AllowNull]
        internal T NewValue { get; }
    }
#nullable disable
}