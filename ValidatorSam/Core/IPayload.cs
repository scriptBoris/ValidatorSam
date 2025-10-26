using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ValidatorSam.Core
{
#nullable enable
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