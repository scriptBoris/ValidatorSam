using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam.Internal
{
    internal interface ISourceRequired
    {
        string GetRequiredError();
    }

    internal class StaticRequired : ISourceRequired
    {
        public StaticRequired(string staticErrorMessage)
        {
            ErrorText = staticErrorMessage;
        }

        public string ErrorText { get; set; }
        public string GetRequiredError() => ErrorText;
    }

    internal class DynamicRequired : ISourceRequired
    {
        public DynamicRequired(Func<string> getError)
        {
            GetErrorFunc = getError;
        }

        public Func<string> GetErrorFunc { get; set; }
        public string GetRequiredError() => GetErrorFunc.Invoke();
    }
}
