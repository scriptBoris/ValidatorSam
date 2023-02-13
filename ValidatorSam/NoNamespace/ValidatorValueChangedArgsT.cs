using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam
{
    public struct ValidatorValueChangedArgs<T>
    {
        public ValidatorValueChangedArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public T OldValue { get; private set; }
        public T NewValue { get; private set; }
    }
}
