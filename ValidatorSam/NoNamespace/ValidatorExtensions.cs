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

        /// <summary>
        /// Manual validation check
        /// </summary>
        /// <param name="validators">Array of validators that will check for valid</param>
        /// <param name="invalids">Detected invalid validators</param>
        /// <returns>True if not detected invalid validators</returns>
        public static bool TryCheckSuccess(this Validator[] validators, out Validator[] invalids)
        {
            bool isValid = true;
            var listInvalids = new List<Validator>();

            foreach (var item in validators)
            {
                var res = item.CheckValid();
                if (!res.IsValid)
                {
                    isValid = false;
                    listInvalids.Add(item);
                }
            }

            invalids = listInvalids.ToArray();
            return isValid;
        }

        public static ValidatorResult? FirstInvalidOrDefault(this Validator[] validators)
        {
            ValidatorResult? result = null;

            foreach (var item in validators)
            {
                var res = item.CheckValid();
                if (!res.IsValid && result == null)
                    result = res;
            }

            return result;
        }

        [Obsolete("Please use FirstInvalidOrDefault")]
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