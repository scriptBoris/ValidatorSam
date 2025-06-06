﻿using System;
using ValidatorSam.Internal;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Class for initialization Validator
    /// </summary>
    public class ValidatorBuilder<T>
    {
        internal const string defaultRequired = "{DEFAULT}";

        internal readonly Validator<T> Validator = new Validator<T>();

        /// <summary>
        /// Internal ctor.
        /// For using Builder with non-Fody way, use <see cref="Validator{T}.BuildManual"/>
        /// </summary>
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

        /// <inheritdoc cref="UsingRule(Func{T, bool}, string)"/>
        /// <param name="rule"></param>
        /// <param name="getError">dynamic function to get error text</param>
        public ValidatorBuilder<T> UsingRule(Func<T, bool> rule, Func<string> getError)
        {
            Validator._rules.Add(new DynamicRuleItem<T>
            {
                DelegateGetError = getError,
                Delegate = rule,
            });
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Validator.IsRequired"/> property to <c>true</c>, indicating that 
        /// <see cref="Validator.Value"/> must not be empty. If this validator contains an empty 
        /// value, the <see cref="Validator.IsValid"/> property will be set to <c>false</c>.
        /// <br/><br/>
        /// <b>Empty values are determined based on the following rules:</b>
        /// </summary>
        /// <remarks>
        /// <para><b>Strings:</b> A value is considered empty if it is <c>null</c>, an 
        /// empty string (<c>""</c>), or consists only of whitespace.</para>
        ///
        /// <para><b>Numbers:</b> Since numeric types (e.g., <see cref="int"/>, 
        /// <see cref="long"/>) are structs and cannot be <c>null</c>, the validator 
        /// determines an empty state using the <see cref="Validator.RawValue"/> property. If 
        /// <see cref="Validator.RawValue"/> is <c>null</c>, an empty string, or a string 
        /// containing only whitespace, it is considered empty.</para>
        ///
        /// <para><b>Other types:</b> A value is considered empty if it is <c>null</c>.</para>
        /// </remarks>
        /// <param name="requiredText">
        /// Custom error message. If omitted, a default error message is automatically 
        /// assigned based on the <see cref="System.Threading.Thread.CurrentCulture"/> setting.
        /// </param>
        public ValidatorBuilder<T> UsingRequired(string requiredText = defaultRequired)
        {
            Validator._required = new StaticRequired(requiredText);
            return this;
        }

        /// <inheritdoc cref="UsingRequired(string)"/>
        /// <param name="getError">dynamic function to get error text</param>
        public ValidatorBuilder<T> UsingRequired(Func<string> getError)
        {
            Validator._required = new DynamicRequired(getError);
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
            Validator.InitValue = value;
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
            object? initValue = builder.Validator.InitValue;
            var hrw = builder.Validator.HandleRaw(null, initValue);

            builder.Validator._value = initValue;
            builder.Validator._rawValue = hrw.newRaw;


            return builder.Validator;
        }
    }
}
#nullable disable