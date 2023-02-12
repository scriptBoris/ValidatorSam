using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam
{
    public struct ValidatorPreprocessArgs
    {
        public string? StringOldValue { get; internal set; }
        public string? StringNewValue { get; internal set; }
        public object? OldValue { get; internal set; }
        public object? NewValue { get; internal set; }
        public bool IsString { get; internal set; }
    }
}
#nullable disable