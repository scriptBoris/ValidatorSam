using System.Diagnostics.CodeAnalysis;

namespace ValidatorSam
{
    /// <summary>
    /// Data container
    /// </summary>
    public readonly struct ValidatorValueChangedArgs
    {
        internal ValidatorValueChangedArgs(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Old value
        /// </summary>
        [AllowNull]
        public object OldValue { get; }

        /// <summary>
        /// New value
        /// </summary>
        [AllowNull] 
        public object NewValue { get; }
    }
}