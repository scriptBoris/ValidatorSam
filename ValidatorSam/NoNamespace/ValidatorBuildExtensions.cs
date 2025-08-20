using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ValidatorSam.Core;
using ValidatorSam.Internal;
using System.Globalization;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Special extension methods for ValidatorBuild
    /// </summary>
    public static class ValidatorBuildExtensions
    {
        /// <summary>
        /// If your will use this rule then rule function will not invoked if new value is null.
        /// <br/>
        /// ---
        /// <br/>
        /// Return <c>false</c> for mark this Validator property IsValid as false
        /// </summary>
        /// <param name="self">builder instance</param>
        /// <param name="safeRule">function that will be called when a new value (not null) is received. If false is returned, an error will be set</param>
        /// <param name="error">the message that will be displayed if the rule function returns false</param>
        public static ValidatorBuilder<T?> UsingSafeRule<T>(this ValidatorBuilder<T?> self, RuleHandler<T> safeRule, string error)
            where T : struct
        {
            RuleHandler<T?> unsafeRule = (args) =>
            {
                var args2 = new RuleArgs<T>(args.Value!.Value, args.RawValue, args.Validator);
                return safeRule(args2);
            };

            self.Validator._rules.Add(new RuleItem<T?>(error, unsafeRule, true));
            return self;
        }

        /// <inheritdoc cref="UsingSafeRule{T}(ValidatorBuilder{T?}, RuleHandler{T}, string)"/>
        public static ValidatorBuilder<T?> UsingSafeRule<T>(this ValidatorBuilder<T?> self, RuleHandler<T> safeRule, Func<string> getError)
            where T : struct
        {
            RuleHandler<T?> unsafeRule = (args) =>
            {
                var args2 = new RuleArgs<T>(args.Value!.Value, args.RawValue, args.Validator);
                return safeRule(args2);
            };

            self.Validator._rules.Add(new DynamicRuleItem<T?>(getError, unsafeRule, true));
            return self;
        }

        /// <summary>
        /// If your will use this rule then rule function will not invoked if new value is null.
        /// <br/>
        /// ---
        /// <br/>
        /// Return <c>false</c> for mark this Validator property IsValid as false
        /// </summary>
        /// <param name="self">builder instance</param>
        /// <param name="safeRule">function that will be called when a new value (not null) is received. If false is returned, an error will be set</param>
        /// <param name="error">the message that will be displayed if the rule function returns false</param>
        public static ValidatorBuilder<T?> UsingSafeRule<T>(this ValidatorBuilder<T?> self, RuleHandler<T> safeRule, string error)
            where T : class
        {
            self.Validator._rules.Add(new RuleItem<T?>(error, (x) => safeRule(x!), true));
            return self;
        }

        /// <inheritdoc cref="UsingSafeRule{T}(ValidatorBuilder{T?}, RuleHandler{T}, string)"/>
        public static ValidatorBuilder<T?> UsingSafeRule<T>(this ValidatorBuilder<T?> self, RuleHandler<T> rule, Func<string> getError)
            where T : class
        {
            self.Validator._rules.Add(new DynamicRuleItem<T?>(getError, (x) => rule(x!), true));
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
                    return PreprocessResult<string>.Ignore();

                string newValueStr = x.NewValue;
                if (newValueStr == "")
                    return PreprocessResult<string>.Ignore();

                int length = newValueStr.Length;
                if (length < min)
                {
                    string msg = string.Format(ValidatorLocalization.Resolve.StringLengthLess, min);
                    return PreprocessResult<string>.Error(msg, newValueStr, null);
                }
                else if (length > max)
                {
                    string msg = string.Format(ValidatorLocalization.Resolve.StringLengthOver, max);
                    return PreprocessResult<string>.Error(msg, newValueStr.Substring(0, (int)max + 1), null);
                }

                return PreprocessResult<string>.Ignore();
            });

            return self;
        }
#nullable enable

        /// <summary>
        /// Creates a preprocessor that limits input values ​​to the ranges min to max.
        /// <br/>
        /// --- 
        /// <br/>
        /// For example, you can use this to specify DateTime, int or other structs constraints
        /// </summary>
        /// <param name="self">builder instance</param>
        /// <param name="min">minimum</param>
        /// <param name="max">maximum</param>
        public static ValidatorBuilder<T> UsingLimitations<T>(this ValidatorBuilder<T> self, [DisallowNull]T min, [DisallowNull]T max)
        {
            self.UsingPreprocessor(x =>
            {
                return CommonLimitations<T>(x, min, max);
            });
            return self;
        }

        private static PreprocessResult<T> CommonLimitations<T>(ValidatorPreprocessArgs<T> arg, [DisallowNull]T min, [DisallowNull] T max)
        {
            if (arg.NewValue is IComparable nc)
            {
                if (nc.CompareTo(min) < 0)
                {
                    string msg = string.Format(ValidatorLocalization.Resolve.StringValueLess, min);
                    return PreprocessResult<T>.Error(msg, min, null);
                }

                if (nc.CompareTo(max) > 0)
                {
                    string msg = string.Format(ValidatorLocalization.Resolve.StringValueOver, max);
                    return PreprocessResult<T>.Error(msg, max, null);
                }
            }

            return PreprocessResult<T>.Ignore();
        }

        /// <summary>
        /// Enables formatting of the value of type T when converting it to a string (RawValue)
        /// <br/>
        /// For example, the format "0.00" will display a double value as 0.00.
        /// </summary>
        /// <param name="self">Builder instance</param>
        /// <param name="format">The string format used to display the value.</param>
        /// <param name="cultureInfo">
        /// The culture to use for formatting. 
        /// Pass null to use CultureInfo.InvariantCulture.
        /// </param>
        public static ValidatorBuilder<T> UsingRawValueFormat<T>(this ValidatorBuilder<T> self, string format, CultureInfo? cultureInfo = null) 
            where T : struct
        {
            self.Validator._stringFormat = format;
            self.Validator._cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
            return self;
        }

        /// <summary>
        /// Enables formatting of the value of type T when converting it to a string (RawValue)
        /// <br/>
        /// For example, the format "0.00" will display a double value as 0.00.
        /// </summary>
        /// <param name="self">Builder instance</param>
        /// <param name="format">The string format used to display the value.</param>
        /// <param name="cultureInfo">
        /// The culture to use for formatting. 
        /// Pass null to use CultureInfo.InvariantCulture.
        /// </param>
        public static ValidatorBuilder<T?> UsingRawValueFormat<T>(this ValidatorBuilder<T?> self, string format, CultureInfo? cultureInfo = null)
            where T : struct
        {
            self.Validator._stringFormat = format;
            self.Validator._cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
            return self;
        }
    }
}
#nullable disable