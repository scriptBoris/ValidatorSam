using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam.Internal
{
    internal interface IRuleItem<T>
    {
        RuleHandler<T> Delegate { get; }
        bool IsSafeRule { get; }
        string GetError();
    }

    internal class RuleItem<T> : IRuleItem<T>
    {
        internal RuleItem(string errorText, RuleHandler<T> rule)
        {
            ErrorText = errorText;
            Delegate = rule;
        }

        internal RuleItem(string errorText, RuleHandler<T> rule, bool isSafeRule)
        {
            ErrorText = errorText;
            Delegate = rule;
            IsSafeRule = isSafeRule;
        }

        internal string ErrorText { get; }
        public RuleHandler<T> Delegate { get; }
        public bool IsSafeRule { get; }

        public string GetError() => ErrorText ?? "";
    }

    internal class DynamicRuleItem<T> : IRuleItem<T>
    {
        internal DynamicRuleItem(Func<string> geterror, RuleHandler<T> delegateGetError)
        {
            DelegateGetError = geterror;
            Delegate = delegateGetError;
        }

        internal DynamicRuleItem(Func<string> geterror, RuleHandler<T> delegateGetError, bool isSafe)
        {
            DelegateGetError = geterror;
            Delegate = delegateGetError;
            IsSafeRule = isSafe;
        }

        internal Func<string> DelegateGetError { get; }
        public RuleHandler<T> Delegate { get; }
        public bool IsSafeRule { get; }

        public string GetError() => DelegateGetError();
    }
}
#nullable disable