using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ValidatorSam.Core
{
#nullable enable
    /// <summary>
    /// Delegate
    /// </summary>
    public delegate void PayloadHandlerAdded(Validator validator, IPayload payload, string key, object added);

    /// <summary>
    /// Delegate
    /// </summary>
    public delegate void PayloadHandlerRemoved(Validator validator, IPayload payload, string key, object removed);

    /// <summary>
    /// Payload container for Validator
    /// </summary>
    public interface IPayload
    {
        /// <summary>
        /// Occurs when a new item is added to the payload
        /// </summary>
        event PayloadHandlerAdded? PayloadAdded;

        /// <summary>
        /// Occurs when an existing item is removed or overwritten
        /// </summary>
        event PayloadHandlerRemoved? PayloadRemoved;

        /// <summary>
        /// Gets the payload value by key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The payload value</returns>
        object GetPayload(string key);

        /// <summary>
        /// Attempts to get the payload value by key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The payload value if found</param>
        /// <returns>True if the key exists; otherwise, false</returns>
        bool TryGetPayload(string key, [NotNullWhen(true)] out object value);

        /// <summary>
        /// Adds a new payload or overwrites an existing one
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The payload value</param>
        void Push(string key, object value);

        /// <summary>
        /// Removes the payload by key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>True if the item was removed; otherwise, false</returns>
        bool Remove(string key);
    }

    internal class Payload : Dictionary<string, object>, IPayload
    {
        private readonly Validator _validator;

        public event PayloadHandlerAdded? PayloadAdded;
        public event PayloadHandlerRemoved? PayloadRemoved;

        public Payload(Validator validator)
        {
            _validator = validator;
        }

        object IPayload.GetPayload(string key)
        {
            return base[key];
        }

        bool IPayload.TryGetPayload(string key, out object value)
        {
            return base.TryGetValue(key, out value);
        }

        void IPayload.Push(string key, object value)
        {
            if (base.Remove(key, out var deleted))
            {
                PayloadRemoved?.Invoke(_validator, this, key, deleted);
            }

            base.Add(key, value);
            PayloadAdded?.Invoke(_validator, this, key, value);
        }

        bool IPayload.Remove(string key)
        {
            if (base.Remove(key, out var deleted))
            {
                PayloadRemoved?.Invoke(_validator, this, key, deleted);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
#nullable disable
}