using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam
{
    /// <summary>
    /// Arguments for validation rule
    /// </summary>
    public readonly struct RuleArgs<T>
    {
        internal RuleArgs(T value, string rawValue, Validator validator)
        {
            Value = value;
            RawValue = rawValue;
            Validator = validator;
        }

        /// <summary>
        /// Value
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Raw value
        /// </summary>
        public string RawValue { get; }

        /// <summary>
        /// The Validator who is invoke current rule
        /// </summary>
        public Validator Validator { get; }
    }
}