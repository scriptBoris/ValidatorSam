using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam.Internal
{
    internal interface IRuleItem<T>
    {
        Func<T, bool> Delegate { get; }
        bool IsSafeRule { get; }
        string GetError();
    }

    internal class RuleItem<T> : IRuleItem<T>
    {
        public Func<T, bool> Delegate { get; set; }
        public string ErrorText { get; set; }
        public bool IsSafeRule { get; set; }

        public string GetError() => ErrorText;
    }

    internal class DynamicRuleItem<T> : IRuleItem<T>
    {
        public Func<T, bool> Delegate { get; set; }
        public bool IsSafeRule { get; set; }
        public Func<string> DelegateGetError { get; set; }

        public string GetError() => DelegateGetError();
    }
}
#nullable disable