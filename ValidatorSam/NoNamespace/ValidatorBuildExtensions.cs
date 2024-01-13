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

        public static ValidatorBuilder<string?> UsingTextLimit(this ValidatorBuilder<string?> self, uint min, uint max)
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
                        //return new PreprocessResult
                        //{
                        //    ErrorText = $"Минимум {min} символов",
                        //    ValueResult = newValueStr,
                        //    ResultType = PreprocessResultType.Error,
                        //};
                    }
                    else if (length > max)
                    {
                        return PreprocessResult.Error($"Максимум {max} символов", newValueStr.Substring(0, (int)max + 1), null);
                        //return new PreprocessResult
                        //{
                        //    ErrorText = $"Максимум {max} символов",
                        //    ValueResult = newValueStr.Substring(0, (int)max + 1),
                        //    ResultType = PreprocessResultType.Error,
                        //};
                    }
                }

                return PreprocessResult.Ignore();
            });

            return self;
        }

        public static ValidatorBuilder<string> UsingSafeTextLimit(this ValidatorBuilder<string> self, uint min, uint max)
        {
            self.UsingTextLimit(min, max);
            return self;
        }


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