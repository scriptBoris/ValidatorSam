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

        public ValidatorBuilder<T> UsingName(string name)
        {
            Validator._customName = name;
            return this;
        }

        public ValidatorBuilder<T> UsingValue(T value)
        {
            usingValue = value;
            return this;
        }

        public ValidatorBuilder<T> UsingEnabledState(bool isEnabled)
        {
            Validator._isEnabled = isEnabled;
            return this;
        }

        public ValidatorBuilder<T> UsingPreprocessor(PreprocessorHandler cast)
        {
            Validator._preprocess.Add(cast);
            return this;
        }

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