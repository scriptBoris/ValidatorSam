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
            set => SetValue(_value, value, ValueInvokers.Value, true, true);
        }

        /// <inheritdoc cref="Validator.RawValue"/>
        public override string RawValue
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
            var res = InternalCheckValid(Value, RawValue ?? "", CheckValidInvokers.External, true, true);
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
            SetValue(_value, value, ValueInvokers.Value, true, true);
        }

        /// <inheritdoc/>
        private string GetRawValue()
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

            return "";
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
            bool canSetValue = true;
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
                    canSetValue = false;
                    break;
                default:
                    throw new NotImplementedException();
            }

            bool notEqualValues = !Equals(_value, result);
            bool notEqualRawValues = !rawValue.SequenceEqual(input);

            if (noError && canSetValue && (notEqualValues || notEqualRawValues))
                SetValue(_value, result, ValueInvokers.RawValue, true, true);

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

        private ValidatorResult ExecuteRule(RuleArgs<T> args, int ruleId)
        {
            var rule = _rules[ruleId];
            bool isValid;

            if (rule.IsSafeRule && args.Value == null)
            {
                isValid = true;
            }
            else
            {
                // If programmer dont catch null, then rule-delegate can throw NullRefException
                isValid = rule.Delegate.Invoke(args);
            }

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
        internal override void SetValue(object? old, object? newest, ValueInvokers invoker, bool useValidations, bool usePreprocessors)
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

            SetValue(oldValue, newValue, invoker, useValidations, usePreprocessors);
        }

        /// <inheritdoc cref="Validator.SetValue"/>
        private void SetValue([AllowNull] T old, [AllowNull] T newest, ValueInvokers invoker, bool useValidations, bool usePreprocessors)
        {
            ValidatorResult? prepError = null;
            T newValue = newest;
            bool updateRaw = invoker == ValueInvokers.Value;

            if (usePreprocessors)
            {
                var hrw = HandlePreprocessor(old, newest);
                switch (hrw.ResultType)
                {
                    case PreprocessResultType.Success:
                        newValue = hrw.NewValue;
                        updateRaw = true;
                        break;
                    case PreprocessResultType.Error:
                        newValue = hrw.NewValue;
                        prepError = hrw.PrepError;
                        updateRaw = true;
                        break;
                    case PreprocessResultType.Ignore:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            _value = newValue;

            if (updateRaw)
            {
                _convertError = null;
                _rawValue = HandleRawDefault(old, newValue);
            }

            if (useValidations)
            {
                ValidatorResult res;
                if (prepError != null)
                    res = prepError.Value;
                else
                    res = InternalCheckValid(_value, _rawValue ?? "", CheckValidInvokers.SetValue, true, false);

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
        /// Преобразует value => value с учетом всех препроцессоров
        /// </summary>
        private HandleRawResult<T> HandlePreprocessor([AllowNull] T old, [AllowNull] T newest)
        {
            if (_preprocess.Count == 0)
                return new HandleRawResult<T>(null, default, PreprocessResultType.Ignore);

            T newValue = newest;
            ValidatorResult prepError = default;
            var args = new ValidatorPreprocessArgs<T>
            {
                Validator = this,
                NewValue = newest,
                OldValue = old,
            };

            int ignoreCount = 0;
            foreach (var preprocessor in _preprocess)
            {
                var pres = preprocessor(args);
                switch (pres.ResultType)
                {
                    case PreprocessResultType.Success:
                        newValue = pres.ValueResult;
                        break;
                    case PreprocessResultType.Error:
                        prepError = new ValidatorResult(false, pres.ErrorText, Name);
                        newValue = pres.ValueResult;
                        return new HandleRawResult<T>(prepError, newValue, PreprocessResultType.Error);
                    case PreprocessResultType.Ignore:
                        ignoreCount++;
                        continue;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (ignoreCount == _preprocess.Count)
                return new HandleRawResult<T>(null, default, PreprocessResultType.Ignore);

            return new HandleRawResult<T>(prepError, newValue, PreprocessResultType.Success);
        }

        private ValidatorResult InternalCheckValid([AllowNull] T genericValue, string rawValue, CheckValidInvokers invoker, bool useValidation, bool usePreprocessors)
        {
            bool isValid = true;
            string? textError = null;
            bool isEmpty = CheckValueIsEmpty(genericValue);

            if (!IsEnabled)
                goto skip;

            // Hight priority
            if (useValidation)
            {
                if (_externalRule != null)
                {
                    var arg = new RuleArgs<object?>(genericValue, rawValue, this);
                    var res = _externalRule(arg);
                    if (!res.IsSuccess)
                    {
                        isValid = false;
                        textError = res.ErrorText;
                        goto skip;
                    }
                }
            }

            if (invoker == CheckValidInvokers.External)
            {
                var parse = TryConvertRawToValue("", RawValue);
                if (parse.ResultType == ConverterResultType.Error)
                {
                    _convertError = parse.ErrorText;
                    isValid = false;
                    textError = _convertError;
                    goto skip;
                }
                else
                {
                    _convertError = null;
                }
            }

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
                var args = new RuleArgs<T>(genericValue!, rawValue, this);
                for (int i = 0; i < RuleCount; i++)
                {
                    var ruleResult = ExecuteRule(args, i);
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
        internal string HandleRawDefault([AllowNull] T old, [AllowNull] T newest)
        {
            var converter = _defaultCastConverter;
            if (converter != null && newest != null)
            {
                string result = converter.ValueToRaw(newest, this);
                return result;
            }
            else
            {
                if (_isGenericStringType)
                {
                    return newest?.ToString() ?? "";
                }

                return "";
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

        /// <summary>
        /// Пытается парсить rawValue (пользовательский ввод) в Generic Type (T)
        /// </summary>
        private ConverterResult<T> TryConvertRawToValue(ReadOnlySpan<char> oldRaw, ReadOnlySpan<char> newRaw)
        {
            if (_defaultCastConverter != null)
            {
                // TODO Очень странный if. В будущем эту логику перенести в конвертеры!
                if (newRaw.Length == 0 && CanNotBeNull)
                    return ConverterResult.Success<T>(default!, newRaw);

                var castResult = _defaultCastConverter.RawToValue(newRaw, oldRaw, _value, this);
                return castResult;
            }
            else
            {
                if (_isGenericStringType)
                {
                    string text = newRaw.ToString() ?? "";
                    if (text is T gtext)
                        return ConverterResult.Success<T>(gtext, newRaw);
                    else
                        throw new InvalidOperationException("Impossible code");
                }
                else
                {
                    return ConverterResult.Ignore<T>();
                }
            }
        }
    }
}
#nullable disable