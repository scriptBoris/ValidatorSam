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
            if (self.isBuilded)
                return self;

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
            if (self.isBuilded)
                return self;

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
            if (self.isBuilded)
                return self;

            self.UsingPreprocessor(x =>
            {
                int l = x.StringNewValue?.Length ?? 0;

                if (l < min)
                {
                    return new PreprocessResult
                    {
                        ErrorText = $"Минимум {min} символов",
                        ValueResult = x.StringNewValue,
                    };
                }
                else if (l > max)
                {
                    return new PreprocessResult
                    {
                        ErrorText = $"Максимум {max} символов",
                        ValueResult = x.StringNewValue?.Substring(0, (int)max + 1)
                    };
                }

                return new PreprocessResult { Type = PreprocessTypeResult.Ignore };
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
            if (self.isBuilded)
                return self;

            self.UsingPreprocessor(x =>
            {
                return CommonLimitations(x, min, max);
            });
            return self;
        }

        public static ValidatorBuilder<T?> UsingLimitations<T>(this ValidatorBuilder<T?> self, T min, T max)
            where T : struct
        {
            if (self.isBuilded)
                return self;

            self.UsingPreprocessor(x =>
            {
                return CommonLimitations(x, min, max);
            });
            return self;
        }

        private static PreprocessResult CommonLimitations(ValidatorPreprocessArgs arg, object min, object max)
        {
            if (arg.NewValue is IComparable nc)
            {
                if (nc.CompareTo(min) < 0)
                {
                    return new PreprocessResult
                    {
                        ErrorText = $"Значение не может быть меньше {min}",
                        ValueResult = min,
                    };
                }

                if (nc.CompareTo(max) > 0)
                {
                    return new PreprocessResult
                    {
                        ErrorText = $"Значение не может быть больше {max}",
                        ValueResult = max,
                    };
                }
            }

            return new PreprocessResult { Type = PreprocessTypeResult.Ignore };
        }
    }
}
#nullable disable