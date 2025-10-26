using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable enable
namespace ValidatorSam
{
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
}
#nullable disable