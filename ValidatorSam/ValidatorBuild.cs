using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ValidatorSam.Core;
using ValidatorSam.Internal;

namespace ValidatorSam
{
    internal class ValidatorContainer
    {
        public object Host { get; set; }
        public Validator Validator { get; set; }
    }

    public partial class Validator
    {
        public static Validator<T> GetOrBuild<T>(ref object instance, [CallerMemberName]string propName = "NONE")
        {
            if (instance == null)
            {
                var validator = new Validator<T>();
                validator.Name = propName;

                instance = validator;
            }


            return (Validator<T>)instance;
        }

        public static Validator[] GetAll(object validatorHoster)
        {
            var list = new List<Validator>();
            var t = validatorHoster.GetType();
            var f = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in f)
            {
                if (item.PropertyType.BaseType == typeof(Validator))
                {
                    var v = item.GetValue(validatorHoster);
                    if (v is Validator validator)
                        list.Add(validator);
                }
                else if (item.PropertyType
                    .GetInterfaces()
                    .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    var genericT = item.PropertyType.GenericTypeArguments.FirstOrDefault();
                    if (genericT == null)
                        continue;

                    if (genericT.GetInterfaces().Any(x => x == typeof(IValidatorHost)))
                    {
                        var v = item.GetValue(validatorHoster);
                        if (v is IEnumerable<IValidatorHost> vv)
                        {
                            foreach (var i in vv)
                            {
                                var ims = GetAll(i);
                                if (ims != null && ims.Length > 0)
                                    list.AddRange(ims);
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }
    }

    public partial class Validator<T> : Validator
    {
        public Validator<T> UsingRule(Func<T, bool> rule, string error)
        {
            _rules.Add(new RuleItem<T>
            {
                ErrorText = error,
                Delegate = rule,
            });
            return this;
        }

        private const string defaultRequired = "{DEFAULT}";
        public Validator<T> UsingRequired(string requiredText = defaultRequired)
        {
            if (requiredText == defaultRequired)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;

                // TODO Need to add more languages
                switch (currentCulture.TwoLetterISOLanguageName)
                {
                    case "en":
                        _requiredText = "Required";
                        break;
                    case "ru":
                        _requiredText = "Обязательно";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _requiredText = requiredText;
            }

            IsRequired = true;
            return this;
        }

        public Validator<T> UsingPreprocessor(Func<ValidatorPreprocessArgs, PreprocessResult> cast)
        {
            _preprocess.Add(cast); 
            return this;
        }
    }
}
