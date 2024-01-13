using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam
{
    public struct ValidatorPreprocessArgs
    {
        public object? OldValue { get; internal set; }
        public object? NewValue { get; internal set; }
    }
}
#nullable disable