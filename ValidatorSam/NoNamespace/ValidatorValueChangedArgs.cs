using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam
{
    public struct ValidatorValueChangedArgs
    {
        public ValidatorValueChangedArgs(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
    }
}
