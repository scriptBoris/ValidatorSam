﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ValidatorSam.Internal;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Main Validator class
    /// </summary>
    /// <typeparam name="T">The type that will appear in checks and preprocessors</typeparam>
    public class Validator<T> : Validator
    {
        internal readonly List<IRuleItem<T>> _rules = new List<IRuleItem<T>>();
        internal readonly List<Action<ValidatorValueChangedArgs<T>>> _changeListeners = new List<Action<ValidatorValueChangedArgs<T>>>();
        private readonly bool _canNotBeNull;

        internal Validator()
        {
            var genericType = typeof(T);
            _isGenericStringType = genericType == typeof(string);
            _canNotBeNull = genericType.IsValueType;
        }

        /// <inheritdoc cref="Validator.Value"/>
        public new T Value
        {
            get
            {
                if (base.Value is T t)
                    return t;
                return default!;
            }
            set => base.Value = value;
        }

        /// <inheritdoc cref="Validator.InitValue"/>
        public new T InitValue
        {
            get
            {
                if (base.InitValue is T t)
                    return t;
                return default!;
            }
            internal set => base.InitValue = value;
        }

        /// <summary>
        /// Internal property
        /// </summary>
        protected override int RuleCount => _rules.Count;

        /// <inheritdoc cref="Validator.CanNotBeNull"/>
        public override bool CanNotBeNull => _canNotBeNull;

        /// <inheritdoc/>
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
                isValid = rule.Delegate.Invoke(t!);

            string? error = !isValid ? rule.GetError() : null;
            return new ValidatorResult(isValid, error, Name);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override object CreateDefaultValue()
        {
            var res = default(T);
            return res!;
        }

        /// <inheritdoc/>
        protected override object? CastValue(object? value)
        {
            if (value is T t)
                return t;
            return default;
        }

        /// <inheritdoc/>
        protected override bool TryCastValue(object? value, out object? cast)
        {
            if (value == null || value == default)
            {
                cast = default(T);
                return false;
            }

            if (value is T t)
            {
                cast = t;
                return true;
            }
            else
            {
                throw new InvalidOperationException("Please use Binding \"RawValue\" instead of \"Value\"");
            }
        }

        /// <inheritdoc cref="Validator.SetValueAsRat(object?, RatModes)"/>
        public void SetValueAsRat(T value, RatModes mode)
        {
            base.SetValueAsRat(value, mode);
        }

        /// <summary>
        /// Internal
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public new void SetValueAsRat(object? value, RatModes mode)
        {
            base.SetValueAsRat(value, mode);
        }

        /// <summary>
        /// Default implicit operator
        /// </summary>
        public static implicit operator T(Validator<T> v)
        {
            return v.Value;
        }

        /// <summary>
        /// Auto-create singleton instance with using Fody code postprocessing
        /// </summary>
        public static ValidatorBuilder<T> Build()
        {
            return new ValidatorBuilder<T>();
        }

        /// <summary>
        /// Manual create singleton instance
        /// </summary>
        public static ValidatorBuilder<T> BuildManual(string propName = "NONE")
        {
            return ValidatorBuilder<T>.Build(propName);
        }
    }
}
#nullable disable