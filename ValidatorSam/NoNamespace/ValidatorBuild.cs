using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ValidatorSam.Core;
using ValidatorSam.Internal;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Class for initialization Validator
    /// </summary>
    public class ValidatorBuilder<T>
    {
        internal readonly Validator<T> Validator = new Validator<T>();
        internal T usingValue;

        internal ValidatorBuilder()
        {
            var genericType = typeof(T);
            ResolveAutoCast(genericType);
        }

        internal static ValidatorBuilder<T> Build(string propName)
        {
            var self = new ValidatorBuilder<T>();
            self.Validator.Name = propName;
            return self;
        }

        /// <summary>
        /// Creates a rule that will perform a validation check every time 
        /// a Value or RawValue is changed or a manual check is performed.
        /// <br/>
        /// ---
        /// <br/>
        /// Return <c>false</c> for mark this Validator property IsValid as false
        /// </summary>
        /// <param name="rule">function that will be called when a new value is received. If false is returned, an error will be set</param>
        /// <param name="error">the message that will be displayed if the rule function returns false</param>
        public ValidatorBuilder<T> UsingRule(Func<T, bool> rule, string error)
        {
            Validator._rules.Add(new RuleItem<T>
            {
                ErrorText = error,
                Delegate = rule,
            });
            return this;
        }

        private const string defaultRequired = "{DEFAULT}";

        /// <summary>
        /// Sets the property IsRequired to true and flag indicating that the property
        /// Value must not be empty. If this validator contains empty value then property 
        /// IsValid will set as false
        /// <br/>
        /// ---
        /// <br/>
        /// For validating types:
        /// <list type="bullet">
        /// <item>
        ///     <term>strings</term>
        ///     <description>
        ///         <c>null</c> or <c>""</c> or white space string is will be considered as empty values
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>numbers</term>
        ///     <description>
        ///         Since types like int32, int64, etc., are struct types and do not support null values, 
        ///         the validator system determines an empty state using the RawValue property. It is 
        ///         assumed that you (the developer) use bindings UI input field to this RawValue 
        ///         property. If so, when RawValue contains null, an empty string, or a string consisting 
        ///         of only whitespace, it will be considered an empty value.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>other types</term>
        ///     <description>
        ///         <c>null</c> will be considered as empty values
        ///     </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="requiredText">
        /// Error message. If used as default then system automaticly setted 
        /// error message based by Thread.CurrentThread.CurrentCulture property
        /// </param>
        public ValidatorBuilder<T> UsingRequired(string requiredText = defaultRequired)
        {
            if (requiredText == defaultRequired)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;

                // TODO Need to add more languages
                switch (currentCulture.TwoLetterISOLanguageName)
                {
                    case "en":
                        Validator._requiredText = "Required";
                        break;
                    case "ru":
                        Validator._requiredText = "Обязательно";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Validator._requiredText = requiredText;
            }

            Validator._isRequired = true;
            return this;
        }

        /// <summary>
        /// Sets the property Name. If not using his method the system automatically chooses name
        /// based by the name of this Validator property
        /// <br/>
        /// ---
        /// <br/>
        /// tip: useful if you want to map the name of an input field UI to an error token from the 
        /// server, for example
        /// </summary>
        /// <param name="name">Name</param>
        public ValidatorBuilder<T> UsingName(string name)
        {
            Validator._customName = name;
            return this;
        }

        /// <summary>
        /// Sets the initial value
        /// </summary>
        /// <param name="value">initial value</param>
        public ValidatorBuilder<T> UsingValue(T value)
        {
            usingValue = value;
            return this;
        }

        /// <summary>
        /// Sets the IsEnabled property
        /// <br/>
        /// ---
        /// <br/>
        /// For more info see <see cref="Validator.IsEnabled"/>
        /// </summary>
        /// <param name="isEnabled">enabled flag</param>
        public ValidatorBuilder<T> UsingEnabledState(bool isEnabled)
        {
            Validator._isEnabled = isEnabled;
            return this;
        }

        /// <summary>
        /// Creates a preprocessor that may will modificated Value before it set
        /// <br/>
        /// ---
        /// <br/>
        /// tip: use it if you want to create a phone mask or something like that.
        /// </summary>
        /// <param name="cast">preprocessor</param>
        public ValidatorBuilder<T> UsingPreprocessor(PreprocessorHandler cast)
        {
            Validator._preprocess.Add(cast);
            return this;
        }

        /// <summary>
        /// Easy to way get callback action when value was change
        /// </summary>
        /// <param name="act">Event args</param>
        public ValidatorBuilder<T> UsingValueChangeListener(Action<ValidatorValueChangedArgs<T>> act)
        {
            Validator._changeListeners.Add(act);
            return this;
        }

        private void ResolveAutoCast(Type type)
        {
            var nullableGenericType = Nullable.GetUnderlyingType(type);
            if (nullableGenericType != null)
                type = nullableGenericType;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    Validator._defaultCast = PreprocessorCollection.CastUint8;
                    break;
                case TypeCode.UInt16:
                    Validator._defaultCast = PreprocessorCollection.CastUint16;
                    break;
                case TypeCode.UInt32:
                    Validator._defaultCast = PreprocessorCollection.CastUint32;
                    break;
                case TypeCode.UInt64:
                    Validator._defaultCast = PreprocessorCollection.CastUint64;
                    break;
                case TypeCode.SByte:
                    Validator._defaultCast = PreprocessorCollection.CastInt8;
                    break;
                case TypeCode.Int16:
                    Validator._defaultCast = PreprocessorCollection.CastInt16;
                    break;
                case TypeCode.Int32:
                    Validator._defaultCast = PreprocessorCollection.CastInt32;
                    break;
                case TypeCode.Int64:
                    Validator._defaultCast = PreprocessorCollection.CastInt64;
                    break;
                case TypeCode.Single:
                    Validator._defaultCast = PreprocessorCollection.CastFloat;
                    break;
                case TypeCode.Double:
                    Validator._defaultCast = PreprocessorCollection.CastDouble;
                    break;
                case TypeCode.Decimal:
                    Validator._defaultCast = PreprocessorCollection.CastDecimal;
                    break;
                default:
                    break;
            }
        }

        public static implicit operator Validator<T>(ValidatorBuilder<T> builder)
        {
            if (builder.usingValue != null)
            {
                builder
                    .Validator
                    .SetValueAsRat(
                        builder.usingValue,
                        RatModes.Default | RatModes.InitValue | RatModes.SkipValidation | RatModes.SkipPreprocessors);
            }
            else if (builder.Validator.CanNotBeNull)
            {
                builder
                    .Validator
                    .SetValueAsRat(
                        default(T),
                        RatModes.Default | RatModes.InitValue | RatModes.SkipValidation);

            }

            return builder.Validator;
        }
    }
}
#nullable disable