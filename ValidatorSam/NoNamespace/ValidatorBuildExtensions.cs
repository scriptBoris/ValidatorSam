using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ValidatorSam.Core;
using ValidatorSam.Internal;

#nullable enable
namespace ValidatorSam
{
    public static class ValidatorBuildExtensions
    {
        /// <summary>
        /// If your will use this rule then rule function will not invoked if new value is null.
        /// <br/>
        /// ---
        /// <br/>
        /// Return false for mark this Validator property IsValid as false
        /// </summary>
        /// <param name="self">builder instance</param>
        /// <param name="rule">function that will be called when a new value (not null) is received. If false is returned, an error will be set</param>
        /// <param name="error">the message that will be displayed if the rule function returns false</param>
        public static ValidatorBuilder<T?> UsingSafeRule<T>(this ValidatorBuilder<T?> self, Func<T, bool> rule, string error)
            where T : struct
        {
            self.Validator._rules.Add(new RuleItem<T?>
            {
                IsSafeRule = true,
                ErrorText = error,
                Delegate = (x) => rule(x.Value),
            });
            return self;
        }

        /// <summary>
        /// If your will use this rule then rule function will not invoked if new value is null.
        /// <br/>
        /// ---
        /// <br/>
        /// Return false for mark this Validator property IsValid as false
        /// </summary>
        /// <param name="self">builder instance</param>
        /// <param name="rule">function that will be called when a new value (not null) is received. If false is returned, an error will be set</param>
        /// <param name="error">the message that will be displayed if the rule function returns false</param>
        public static ValidatorBuilder<T?> UsingSafeRule<T>(this ValidatorBuilder<T?> self, Func<T, bool> rule, string error)
            where T : class
        {
            self.Validator._rules.Add(new RuleItem<T?>
            {
                IsSafeRule = true,
                ErrorText = error,
                Delegate = (x) => rule(x),
            });
            return self;
        }

#nullable disable
        /// <summary>
        /// Creates a preprocessor that allows input text is shorter than the min length, but 
        /// restricts text entry if it is longer than the max length.
        /// <br/>
        /// --- 
        /// <br/>
        /// Any way if input was out of limits, the Validator will show default error message and
        /// mark the property IsValid as false
        /// <br/>
        /// For null values this preprocessor will did ignored
        /// </summary>
        /// <param name="self">builder instance</param>
        /// <param name="min">minimum text length</param>
        /// <param name="max">maximum text length</param>
        public static ValidatorBuilder<string> UsingTextLimit(this ValidatorBuilder<string> self, uint min, uint max)
        {
            self.UsingPreprocessor(x =>
            {
                if (x.NewValue == null)
                    return PreprocessResult.Ignore();

                if (x.NewValue is string newValueStr)
                {
                    int length = newValueStr.Length;
                    if (length < min)
                    {
                        return PreprocessResult.Error($"Минимум {min} символов", newValueStr, null);
                    }
                    else if (length > max)
                    {
                        return PreprocessResult.Error($"Максимум {max} символов", newValueStr.Substring(0, (int)max + 1), null);
                    }
                }

                return PreprocessResult.Ignore();
            });

            return self;
        }
#nullable enable

        public static ValidatorBuilder<T> UsingLimitations<T>(this ValidatorBuilder<T> self, T min, T max)
            where T : struct
        {
            self.UsingPreprocessor(x =>
            {
                return CommonLimitations(x, min, max);
            });
            return self;
        }

        public static ValidatorBuilder<T?> UsingLimitations<T>(this ValidatorBuilder<T?> self, T min, T max)
            where T : struct
        {
            self.UsingPreprocessor(x =>
            {
                return CommonLimitations(x, min, max);
            });
            return self;
        }

        private static PreprocessResult CommonLimitations(ValidatorPreprocessArgs arg, object min, object max)
        {
            if (arg.NewValue is IComparable nc && arg.NewValue.GetType() == min.GetType())
            {
                if (nc.CompareTo(min) < 0)
                {
                    return PreprocessResult.Error($"Значение не может быть меньше {min}", min, null);
                    //return new PreprocessResult
                    //{
                    //    ErrorText = $"Значение не может быть меньше {min}",
                    //    ResultType = PreprocessResultType.Error,
                    //    ValueResult = min,
                    //};
                }

                if (nc.CompareTo(max) > 0)
                {
                    return PreprocessResult.Error($"Значение не может быть больше {max}", max, null);
                    //return new PreprocessResult
                    //{
                    //    ErrorText = $"Значение не может быть больше {max}",
                    //    ResultType= PreprocessResultType.Error,
                    //    ValueResult = max,
                    //};
                }
            }

            return PreprocessResult.Ignore();
        }
    }
}
#nullable disable