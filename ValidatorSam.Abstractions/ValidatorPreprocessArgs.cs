using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Data container
    /// </summary>
    public struct ValidatorPreprocessArgs<T>
    {
        /// <summary>
        /// Invoker
        /// </summary>
        public IValidator Validator { get; set; }

        /// <summary>
        /// Old user input typed text
        /// </summary>
        public string OldRawValue { get; set; }

        /// <summary>
        /// New user input typed text
        /// </summary>
        public string NewRawValue { get; set; }

        /// <summary>
        /// Old validated value
        /// </summary>
        [AllowNull]
        public T OldValue { get; set; }

        /// <summary>
        /// New value if generic type of Validator is not String
        /// </summary>
        [AllowNull]
        public T NewValue { get; set; }
    }
}
#nullable disable