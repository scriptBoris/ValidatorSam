using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam.Core
{
    public enum PreprocessResultType
    {
        Success,
        Error,
        Ignore,
    }

    public class PreprocessResult
    {
        private PreprocessResult()
        {
        }

        public PreprocessResultType ResultType { get; private set; }
        public string? ErrorText { get; private set; }
        public object? ValueResult { get; private set; }
        public string? RawResult { get; private set; }

        public static PreprocessResult Success(object? value, string? raw)
        {
            return new PreprocessResult
            {
                ResultType = PreprocessResultType.Success,
                ValueResult = value,
                RawResult = raw,
            };
        }

        public static PreprocessResult Error(string errorText, object? value, string? raw)
        {
            return new PreprocessResult
            {
                ResultType = PreprocessResultType.Error,
                ErrorText = errorText,
                ValueResult = value,
                RawResult = raw,
            };
        }

        public static PreprocessResult Ignore()
        {
            return new PreprocessResult() { ResultType = PreprocessResultType.Ignore };
        }
    }
}
#nullable disable