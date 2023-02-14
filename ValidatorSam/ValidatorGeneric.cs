using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidatorSam.Core;
using ValidatorSam.Internal;

#nullable enable
namespace ValidatorSam
{
    public class Validator<T> : Validator
    {
        internal readonly List<RuleItem<T>> _rules = new List<RuleItem<T>>();
        internal readonly List<Action<ValidatorValueChangedArgs<T>>> _changeListeners = new List<Action<ValidatorValueChangedArgs<T>>>();
        private readonly bool _canNotBeNull;

        internal Validator()
        {
            var genericType = typeof(T);
            _isGenericStringType = genericType == typeof(string);
            _canNotBeNull = genericType.IsValueType;

            if (_canNotBeNull)
                SetValueAsRat(default(T));
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

        protected override void ThrowValueChangeListener(object? oldValue, object? newValue)
        {
            if (oldValue is T tOld) { }
            else { 
                tOld = default(T);
            }

            if (newValue is T tNew) { }
            else {
                tNew = default(T);
            }

            foreach (var item in _changeListeners)
            {
                item.Invoke(new ValidatorValueChangedArgs<T>(tOld!, tNew!));
            }
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

        public static implicit operator T(Validator<T> v)
        {
            return v.Value;
        }
    }
}
#nullable disable