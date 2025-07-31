using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ValidatorSam.Internal
{
#nullable enable
    internal struct HandleRawResult<T>
    {
        /// <inheritdoc cref="HandleRawResult.PrepError"/>
        public ValidatorResult? PrepError { get; set; }

        /// <inheritdoc cref="HandleRawResult.NewRaw"/>
        public string? NewRaw { get; set; }

        /// <inheritdoc cref="HandleRawResult.NewValue"/>
        [AllowNull]
        public T NewValue { get; set; }

        /// <inheritdoc cref="HandleRawResult.ForceUpdateRaw"/>
        public bool ForceUpdateRaw { get; set; }
    }

    internal struct HandleRawResult
    {
        /// <summary>
        /// Ошибка препроцессора
        /// </summary>
        public ValidatorResult? PrepError { get; set; }

        /// <summary>
        /// Текстовое представление значения (RawValue)
        /// </summary>
        public string? NewRaw { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public object? NewValue { get; set; }

        /// <summary>
        /// Нужно ли форсировано перерисовать RawValue
        /// </summary>
        public bool ForceUpdateRaw { get; set; }
    }
#nullable disable
}