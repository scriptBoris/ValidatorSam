using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam
{
    public static class ValidatorExtensions
    {
        public static T ValueOrDefault<T>(this Validator<T?> validator)
            where T : struct
        {
            if (validator.Value == null)
                return default(T);

            return validator.Value.Value;
        }

        public static bool CheckSuccess(this Validator[] validators)
        {
            var result = new ValidatorResult(true, null, "none");

            foreach (var item in validators)
            {
                var res = item.CheckValid();
                if (!res.IsValid)
                    result = res;
            }

            return result.IsValid;
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
#nullable enable