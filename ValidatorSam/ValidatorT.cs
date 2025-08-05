using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ValidatorSam.Internal;
using ValidatorSam.Core;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;

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
        internal readonly List<PreprocessorHandler<T>> _preprocess = new List<PreprocessorHandler<T>>();
        private readonly bool _canNotBeNull;

        internal IValueRawConverter<T>? _defaultCastConverter;

        [AllowNull]
        internal T _value;
        internal string? _rawValue;
        internal string? _stringFormat;
        internal CultureInfo? _cultureInfo;
        private string? _convertError;
        private bool _isUnlockedVisualValid;

        internal Validator()
        {
            var genericType = typeof(T);
            _isGenericStringType = genericType == typeof(string);
            _canNotBeNull = genericType.IsValueType;
        }

        /// <inheritdoc cref="Validator.Value"/>
        public new T Value
        {
            get => _value;
            set => SetValue(_value, value, ValueInvokers.Value, true, true, true);
        }

        /// <inheritdoc cref="Validator.RawValue"/>
        public override string? RawValue
        {
            get => GetRawValue();
            set => SetRawValue(value);
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

        /// <inheritdoc/>
        public override string? StringFormat
        {
            get => _stringFormat;
            set
            {
                _stringFormat = value;
                _defaultCastConverter?.OnStringFormartChanged(value);
            }
        }

        /// <inheritdoc/>
        public override CultureInfo? CultureInfo
        {
            get => _cultureInfo;
            set
            {
                _cultureInfo = value;
                _defaultCastConverter?.OnCultureInfoChanged(value);
            }
        }

        /// <inheritdoc/>
        public override bool IsVisualValid
        {
            get
            {
                if (!_isUnlockedVisualValid)
                    return true;
                else
                    return IsValid && TextError == null;
            }
        }

        /// <summary>
        /// Internal property
        /// </summary>
        protected override int RuleCount => _rules.Count;

        /// <inheritdoc cref="Validator.CanNotBeNull"/>
        public override bool CanNotBeNull => _canNotBeNull;

        /// <inheritdoc/>
        public override bool HasValue => !CheckValueIsEmpty(Value);

        /// <inheritdoc/>
        public override ValidatorResult CheckValid()
        {
            var res = InternalCheckValid(Value, true, true);
            _isUnlockedVisualValid = true;
            IsValid = res.IsValid;
            TextError = res.TextError;
            InvokeErrorChanged(this, ValidatorErrorTextArgs.Calc(!res.IsValid, res.TextError));
            //ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Calc(!res.IsValid, res.TextError));
            return res;
        }

        /// <inheritdoc/>
        public override void SetError(string textError)
        {
            _isUnlockedVisualValid = true;
            IsValid = false;
            TextError = textError;
            InvokeErrorChanged(this, ValidatorErrorTextArgs.Calc(true, textError));
            OnPropertyChanged(nameof(IsVisualValid));
        }

        /// <inheritdoc/>
        protected override object? GenericGetValue()
        {
            return _value;
        }

        /// <inheritdoc/>
        protected override void GenericSetValue(object? value)
        {
            SetValue(_value, value, ValueInvokers.Value, true, true, true);
        }

        /// <inheritdoc/>
        private string? GetRawValue()
        {
            if (_rawValue != null)
            {
                return _rawValue;
            }

            if (_value != null)
            {
                if (_value is IFormattable fvalue && _stringFormat != null)
                {
                    return fvalue.ToString(_stringFormat, CultureInfo.InvariantCulture);
                }
                else
                {
                    return _value.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Устанавливает RawValue (_rawValue)
        /// </summary>
        private void SetRawValue(ReadOnlySpan<char> value)
        {
            var rawValue = _rawValue != null ? _rawValue.AsSpan() : ReadOnlySpan<char>.Empty;
            var input = value;

            if (rawValue.SequenceEqual(input))
                return;

            var convertResult = TryConvertRawToValue(rawValue, value);
            bool noError;
            T result;
            switch (convertResult.ResultType)
            {
                case ConverterResultType.Success:
                    _convertError = null;
                    _rawValue = convertResult.RawResult;
                    result = convertResult.Result;
                    noError = true;
                    break;
                case ConverterResultType.Error:
                    _convertError = convertResult.ErrorText ?? "NO_ERROR_TEXT";
                    _rawValue = convertResult.RawResult;
                    result = default;
                    _value = default;
                    SetError(_convertError);
                    noError = false;
                    break;
                case ConverterResultType.Skip:
                    _convertError = null;
                    _rawValue = value.ToString();
                    result = default;
                    noError = true;
                    break;
                default:
                    throw new NotImplementedException();
            }

            bool notEqualValues = !Equals(_value, result);
            bool notEqualRawValues = !rawValue.SequenceEqual(input);

            if (noError && (notEqualValues || notEqualRawValues))
                SetValue(_value, result, ValueInvokers.RawValue, false, true, true);

            if (notEqualRawValues)
                OnPropertyChanged(nameof(RawValue));
        }

        /// <summary>
        /// Check value is empty or not
        /// </summary>
        protected bool CheckValueIsEmpty([AllowNull] T genericValue)
        {
            bool isEmpty;

            switch (genericValue)
            {
                case string vstring:
                    isEmpty = string.IsNullOrWhiteSpace(vstring);
                    break;
                case bool vbool:
                    isEmpty = !vbool;
                    break;
                default:
                    isEmpty = genericValue == null;
                    break;
            }

            return isEmpty;
        }

        /// <inheritdoc/>
        protected ValidatorResult ExecuteRule([AllowNull] T value, int ruleId)
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
        protected void ThrowValueChangeListener([AllowNull] T oldValue, [AllowNull] T newValue)
        {
            if (oldValue is T tOld) { }
            else
            {
                tOld = default(T);
            }

            if (newValue is T tNew) { }
            else
            {
                tNew = default(T);
            }

            var args = new ValidatorValueChangedArgs<T>(tOld, tNew);
            foreach (var item in _changeListeners)
            {
                item.Invoke(args);
            }
        }

        /// <inheritdoc/>
        internal override void SetValue(object? old, object? newest, ValueInvokers invoker, bool updateRaw, bool useValidations, bool usePreprocessors)
        {
            T oldValue;
            if (old is T castOld)
                oldValue = castOld;
            else
                oldValue = default;

            T newValue;
            if (newest is T castNew)
                newValue = castNew;
            else
                newValue = default;

            SetValue(oldValue, newValue, invoker, updateRaw, useValidations, usePreprocessors);
        }

        /// <inheritdoc cref="Validator.SetValue"/>
        private void SetValue([AllowNull] T old, [AllowNull] T newest, ValueInvokers invoker, bool updateRaw, bool useValidations, bool usePreprocessors)
        {
            ValidatorResult? prepError = null;
            string? newRaw = null;
            T newValue = newest;
            bool forceUpdateRaw = false;
            if (usePreprocessors)
            {
                var hrw = HandleRaw(old, newest);
                prepError = hrw.PrepError;
                newRaw = hrw.NewRaw;
                newValue = hrw.NewValue;
                forceUpdateRaw = hrw.ForceUpdateRaw;
            }

            _value = newValue;

            if (updateRaw || forceUpdateRaw)
                _rawValue = newRaw;

            if (invoker == ValueInvokers.Value)
                _convertError = null;

            if (useValidations)
            {
                ValidatorResult res;
                if (prepError != null)
                    res = prepError.Value;
                else
                    res = InternalCheckValid(_value, true, false);

                bool oldValid = IsValid;
                string? oldTextError = TextError;

                _isUnlockedVisualValid = true;
                if (oldValid != res.IsValid)
                {
                    IsValid = res.IsValid;
                    InvokeValidationChanged(this, IsValid);
                }

                if (oldTextError != res.TextError)
                {
                    TextError = res.TextError;
                    InvokeErrorChanged(this, ValidatorErrorTextArgs.Calc(!res.IsValid, res.TextError));
                }

                OnPropertyChanged(nameof(IsVisualValid));
            }

            if (!Equals(old, _value))
            {
                InvokeValueChanged(this, new ValidatorValueChangedArgs(old, _value));
                ThrowValueChangeListener(old, _value);
                OnPropertyChanged(nameof(Value));
            }

            if (updateRaw)
                OnPropertyChanged(nameof(RawValue));
        }

        /// <summary>
        /// Преобразует value => raw value с учетом препроцессоров
        /// </summary>
        private HandleRawResult<T> HandleRaw([AllowNull] T old, [AllowNull] T newest)
        {
            ValidatorResult? prepError = null;
            string? newRaw = null;
            T newValue = newest;
            bool forceUpdateRaw = false;
            var args = new ValidatorPreprocessArgs<T>
            {
                Validator = this,
                NewValue = newest,
                OldValue = old,
            };
            foreach (var preprocessor in _preprocess)
            {
                var pres = preprocessor(args);
                switch (pres.ResultType)
                {
                    case PreprocessResultType.Success:
                        newRaw = pres.RawResult;
                        newValue = pres.ValueResult;
                        break;
                    case PreprocessResultType.Error:
                        prepError = new ValidatorResult(false, pres.ErrorText, Name);
                        newRaw = pres.RawResult;
                        newValue = pres.ValueResult;
                        forceUpdateRaw = true;
                        break;
                    case PreprocessResultType.Ignore:
                        continue;
                    default:
                        throw new NotImplementedException();
                }
            }

            return new HandleRawResult<T>
            {
                PrepError = prepError,
                NewRaw = newRaw,
                NewValue = newValue,
                ForceUpdateRaw = forceUpdateRaw,
            };
        }

        private ValidatorResult InternalCheckValid([AllowNull] T genericValue, bool useValidation, bool usePreprocessors)
        {
            bool isValid = true;
            string? textError = null;
            bool isEmpty = CheckValueIsEmpty(genericValue);

            if (!IsEnabled)
                goto skip;

            if (IsRequired)
            {
                if (isEmpty)
                {
                    isValid = false;
                    string requiredText = _required!.GetRequiredError();

                    if (requiredText == ValidatorBuilder<object>.defaultRequired)
                        textError = ValidatorLocalization.Resolve.StringRequired;
                    else
                        textError = requiredText;

                    goto skip;
                }
            }

            if (_convertError != null)
            {
                isValid = false;
                textError = _convertError;
                goto skip;
            }

            if (usePreprocessors && !isEmpty)
            {
                foreach (var item in _preprocess)
                {
                    var res = item(new ValidatorPreprocessArgs<T>
                    {
                        OldValue = _value,
                        NewValue = genericValue,
                    });

                    if (res.ResultType == PreprocessResultType.Error)
                        return new ValidatorResult(false, res.ErrorText, Name);
                }
            }

            if (useValidation)
            {
                for (int i = 0; i < RuleCount; i++)
                {
                    var ruleResult = ExecuteRule(genericValue, i);
                    if (!ruleResult.IsValid)
                    {
                        isValid = false;
                        textError = ruleResult.TextError;
                        break;
                    }
                }
            }

        skip:
            return new ValidatorResult(isValid, textError, Name);
        }

        /// <summary>
        /// Преобразует Value в текстовое представление для RawValue
        /// (например для цифр, int, double и т.д.)
        /// </summary>
        internal HandleRawResult<T> HandleRawDefault([AllowNull] T old, [AllowNull] T newest)
        {
            ValidatorResult? prepError = null;
            string? newRaw = null;
            T newValue = default;

            bool forceUpdateRaw = false;
            var converter = _defaultCastConverter;
            if (converter != null)
            {
                var result = converter.ValueToRaw(newest, old, this);
                switch (result.ResultType)
                {
                    case ConverterResultType.Success:
                        newRaw = result.RawResult;
                        newValue = result.Result;
                        break;
                    case ConverterResultType.Error:
                        prepError = new ValidatorResult(false, result.ErrorText, Name);
                        newRaw = result.RawResult;
                        newValue = result.Result;
                        forceUpdateRaw = true;
                        break;
                    case ConverterResultType.Skip:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                newRaw = newest?.ToString();
                newValue = newest;
            }

            return new HandleRawResult<T>
            {
                PrepError = prepError,
                NewRaw = newRaw,
                NewValue = newValue,
                ForceUpdateRaw = forceUpdateRaw,
            };
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

        /// <summary>
        /// Пытается парсить rawValue (пользовательский ввод) в Generic Type (T)
        /// </summary>
        private ConverterResult<T> TryConvertRawToValue(ReadOnlySpan<char> oldRaw, ReadOnlySpan<char> newRaw)
        {
            if (_defaultCastConverter != null)
            {
                var castResult = _defaultCastConverter.RawToValue(newRaw, oldRaw, _value, this);
                return castResult;
            }
            else
            {
                return ConverterResult.Ignore<T>();
            }
        }
    }
}
#nullable disable