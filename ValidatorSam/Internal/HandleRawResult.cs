using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ValidatorSam.Internal
{
#nullable enable
    internal readonly struct HandleRawResult<T>
    {
        internal HandleRawResult(
            ValidatorResult? prepError,
            string? newRaw,
            [AllowNull] T newValue,
            bool forceUpdateRaw)
        {
            PrepError = prepError;
            NewRaw = newRaw;
            NewValue = newValue;
            ForceUpdateRaw = forceUpdateRaw;
        }

        /// <summary>
        /// Ошибка препроцессора
        /// </summary>
        internal ValidatorResult? PrepError { get; }

        /// <summary>
        /// Текстовое представление значения (RawValue)
        /// </summary>
        internal string? NewRaw { get; }

        /// <summary>
        /// Значение
        /// </summary>
        [AllowNull]
        internal T NewValue { get; }

        /// <summary>
        /// Нужно ли форсировано перерисовать RawValue
        /// </summary>
        internal bool ForceUpdateRaw { get; }
    }
#nullable disable
}