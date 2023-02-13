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
        private ValidatorBuilder()
        {
        }

        internal readonly Validator<T> Validator = new Validator<T>();
        internal bool isBuilded;

        public static ValidatorBuilder<T> Build(ref object? instance, [CallerMemberName] string propName = "NONE")
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

    //public partial class Validator
    //{
    //    [Obsolete("NO USE!")]
    //    public static Validator<T> GetOrBuild<T>(ref object instance, [CallerMemberName] string propName = "NONE")
    //    {
    //        if (instance == null)
    //        {
    //            var validator = new Validator<T>();
    //            validator.Name = propName;

    //            instance = validator;
    //        }


    //        return (Validator<T>)instance;
    //    }

    //    public static Validator<T> Build<T>([CallerMemberName] string propName = "NONE")
    //    {
    //        var validator = new Validator<T>();
    //        validator.Name = propName;
    //        return validator;
    //    }

    //    public static Validator[] GetAll(object validatorHoster)
    //    {
    //        var list = new List<Validator>();
    //        var t = validatorHoster.GetType();
    //        var f = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //        foreach (var item in f)
    //        {
    //            if (item.PropertyType.BaseType == typeof(Validator))
    //            {
    //                var v = item.GetValue(validatorHoster);
    //                if (v is Validator validator)
    //                    list.Add(validator);
    //            }
    //            else if (item.PropertyType
    //                .GetInterfaces()
    //                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
    //            {
    //                var genericT = item.PropertyType.GenericTypeArguments.FirstOrDefault();
    //                if (genericT == null)
    //                    continue;

    //                if (genericT.GetInterfaces().Any(x => x == typeof(IValidatorHost)))
    //                {
    //                    var v = item.GetValue(validatorHoster);
    //                    if (v is IEnumerable<IValidatorHost> vv)
    //                    {
    //                        foreach (var i in vv)
    //                        {
    //                            var ims = GetAll(i);
    //                            if (ims != null && ims.Length > 0)
    //                                list.AddRange(ims);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return list.ToArray();
    //    }
    //}

    //public partial class Validator<T> : Validator
    //{
    //    public Validator<T> UsingRule(Func<T, bool> rule, string error)
    //    {
    //        _rules.Add(new RuleItem<T>
    //        {
    //            ErrorText = error,
    //            Delegate = rule,
    //        });
    //        return this;
    //    }

    //    private const string defaultRequired = "{DEFAULT}";
    //    public Validator<T> UsingRequired(string requiredText = defaultRequired)
    //    {
    //        if (requiredText == defaultRequired)
    //        {
    //            var currentCulture = Thread.CurrentThread.CurrentCulture;

    //            // TODO Need to add more languages
    //            switch (currentCulture.TwoLetterISOLanguageName)
    //            {
    //                case "en":
    //                    _requiredText = "Required";
    //                    break;
    //                case "ru":
    //                    _requiredText = "Обязательно";
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //        else
    //        {
    //            _requiredText = requiredText;
    //        }

    //        IsRequired = true;
    //        return this;
    //    }

    //    public Validator<T> UsingEnabledState(bool isEnabled)
    //    {
    //        _isEnabled = isEnabled;
    //        return this;
    //    }

    //    public Validator<T> UsingPreprocessor(Func<ValidatorPreprocessArgs, PreprocessResult> cast)
    //    {
    //        _preprocess.Add(cast);
    //        return this;
    //    }
    //}
}
#nullable disable