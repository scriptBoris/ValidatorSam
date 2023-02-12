using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ValidatorSam.Internal;

namespace ValidatorSam
{
    public static class ValidatorExtensions
    {
        public static Validator<T?> UsingSafeRule<T>(this Validator<T?> self, Func<T, bool> rule, string error)
            where T : struct
        {
            self._rules.Add(new RuleItem<T?>
            {
                IsSafeRule = true,
                ErrorText = error,
                Delegate = (x) => rule(x.Value),
            });
            return self;
        }

        public static ValidatorResult FirstInvalid(this Validator[] validators)
        {
            var result = new ValidatorResult(true, null, "none");

            foreach (var item in validators)
            {
                var res = item.CheckValid();
                if (!res.IsValid)
                    result = res;
            }

            return result;
        }
    }
}
