using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam.Core
{
    public enum PreprocessTypeResult
    {
        Success,
        Error,
        Ignore,
    }

    public struct PreprocessResult
    {
        public PreprocessTypeResult Type { get; set; }
        public string? ErrorText { get; set; }
        public object? ValueResult { get; set; }
        public string? TextResult { get; set; }

        public ValidatorPreprocessArgs AsArg(ValidatorPreprocessArgs last)
        {
            bool isString = false;
            if (ValueResult is string)
                isString = true;

            return new ValidatorPreprocessArgs
            {
                IsString = isString,
                NewValue = ValueResult,
                OldValue = last.NewValue,
                StringNewValue = TextResult,
                StringOldValue = last.StringNewValue,
            };
        }
    }
}
#nullable disable