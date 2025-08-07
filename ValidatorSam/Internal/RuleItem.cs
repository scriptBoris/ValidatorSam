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
        internal RuleItem(string errorText, Func<T, bool> rule)
        {
            ErrorText = errorText;
            Delegate = rule;
        }

        internal RuleItem(string errorText, Func<T, bool> rule, bool isSafeRule)
        {
            ErrorText = errorText;
            Delegate = rule;
            IsSafeRule = isSafeRule;
        }

        internal string ErrorText { get; }
        public Func<T, bool> Delegate { get; }
        public bool IsSafeRule { get; }

        public string GetError() => ErrorText ?? "";
    }

    internal class DynamicRuleItem<T> : IRuleItem<T>
    {
        internal DynamicRuleItem(Func<string> geterror, Func<T, bool> delegateGetError)
        {
            DelegateGetError = geterror;
            Delegate = delegateGetError;
        }

        internal DynamicRuleItem(Func<string> geterror, Func<T, bool> delegateGetError, bool isSafe)
        {
            DelegateGetError = geterror;
            Delegate = delegateGetError;
            IsSafeRule = isSafe;
        }

        internal Func<string> DelegateGetError { get; }
        public Func<T, bool> Delegate { get; }
        public bool IsSafeRule { get; }

        public string GetError() => DelegateGetError();
    }
}
#nullable disable