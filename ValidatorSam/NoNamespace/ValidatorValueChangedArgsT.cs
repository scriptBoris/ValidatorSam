using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ValidatorSam
{
#nullable enable
    /// <summary>
    /// Data wrapper for ValidatorValueListened
    /// </summary>
    public struct ValidatorValueChangedArgs<T>
    {
        internal ValidatorValueChangedArgs([AllowNull]T oldValue, [AllowNull] T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Old validated value
        /// </summary>
        [AllowNull]
        public T OldValue { get; private set; }

        /// <summary>
        /// New validated value
        /// </summary>
        [AllowNull]
        public T NewValue { get; private set; }
    }
#nullable disable
}
