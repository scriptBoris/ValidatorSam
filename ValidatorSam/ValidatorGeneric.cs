using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidatorSam.Core;
using ValidatorSam.Internal;

#nullable enable
namespace ValidatorSam
{
    public partial class Validator<T> : Validator
    {
        internal readonly List<RuleItem<T>> _rules = new List<RuleItem<T>>();
        private readonly bool _isGenericStringType;
        private readonly bool _canNotBeNull;

        public Validator()
        {
            var genericType = typeof(T);
            _isGenericStringType = genericType == typeof(string);
            _canNotBeNull = genericType.IsValueType;
            ResolveAutoCast(genericType);

            if (_canNotBeNull)
                SetValueAsRat(default(T));
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

        public new T Value
        {
            get
            {
                if ((this as Validator).Value is T t)
                    return t;
                return default!;
            }
            set => (this as Validator).Value = value;
        }

        protected override int RuleCount => _rules.Count;
        protected override bool CanNotBeNull => _canNotBeNull;

        protected override ValidatorResult ExecuteRule(object? value, int ruleId)
        {
            // Ugly construction :Q
            // Compatability NET2.1
            if (value is T t) { }
            else
            {
                if (_isGenericStringType && "" is T st)
                    t = st;
                else
                    t = default;
            }

            var rule = _rules[ruleId];
            bool isValid;

            if (rule.IsSafeRule && t == null)
                isValid = true;
            else
                // If programmer dont catch null, then rule-delegate can throw NullRefException
                isValid = !rule.Delegate.Invoke(t!);

            string? error = !isValid ? rule.ErrorText : null;
            return new ValidatorResult(isValid, error, Name);
        }

        protected override object CreateDefaultValue()
        {
            var res = default(T);
            return res!;
        }

        protected override object? CastValue(object? value)
        {
            if (value is T t)
                return t;
            return default;
        }

        protected override bool TryCastValue(object? value, out object? cast)
        {
            if (value is T t)
            {
                cast = t;
                return true;
            }
            else
            {
                cast = default(T);
                return false;
            }
        }
    }
}
#nullable disable