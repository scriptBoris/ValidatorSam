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
    public class ValidatorBuilder<T>
    {
        internal readonly Validator<T> Validator = new Validator<T>();
        internal bool isBuilded;

        internal ValidatorBuilder()
        {
        }

        internal static ValidatorBuilder<T> Build(ref object? instance, string propName = "NONE")
        {
            if (instance == null)
            {
                var self = new ValidatorBuilder<T>();
                self.Validator.Name = propName;

                var genericType = typeof(T);
                self.ResolveAutoCast(genericType);

                instance = self;
                return self;
            }
            else
            {
                var self = (ValidatorBuilder<T>)instance;
                self.isBuilded = true;
                return self;
            }
        }

        public ValidatorBuilder<T> UsingRule(Func<T, bool> rule, string error)
        {
            if (isBuilded)
                return this;

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
            if (isBuilded)
                return this;

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

        public ValidatorBuilder<T> UsingValue(T value)
        {
            if (isBuilded)
                return this;

            Validator.SetValueAsRat(value, true);
            return this;
        }

        public ValidatorBuilder<T> UsingEnabledState(bool isEnabled)
        {
            if (isBuilded)
                return this;

            Validator._isEnabled = isEnabled;
            return this;
        }

        public ValidatorBuilder<T> UsingPreprocessor(Func<ValidatorPreprocessArgs, PreprocessResult> cast)
        {
            if (isBuilded)
                return this;

            Validator._preprocess.Add(cast);
            return this;
        }

        public ValidatorBuilder<T> UsingValueChangeListener(Action<ValidatorValueChangedArgs<T>> act)
        {
            if (isBuilded)
                return this;

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
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                    break;
                case TypeCode.Int32:
                    UsingPreprocessor(PreprocessorCollection.CastInt32);
                    break;
                case TypeCode.Int64:
                case TypeCode.Decimal:
                    break;
                case TypeCode.Double:
                    UsingPreprocessor(PreprocessorCollection.CastDouble);
                    break;
                case TypeCode.Single:
                    break;
                default:
                    break;
            }
        }

        public static implicit operator Validator<T>(ValidatorBuilder<T> t)
        {
            return t.Validator;
        }
    }
}
#nullable disable