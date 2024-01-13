using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ValidatorSam.Core;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Default delegate validator
    /// </summary>
    public delegate void EventHandlerSure<T>(Validator invoker, T args);

    /// <summary>
    /// Delegate for preprocess input value
    /// </summary>
    public delegate PreprocessResult PreprocessorHandler(ValidatorPreprocessArgs args);

    /// <summary>
    /// Base class validator
    /// </summary>
    public abstract class Validator : INotifyPropertyChanged
    {
        internal readonly List<PreprocessorHandler> _preprocess = new List<PreprocessorHandler>();
        internal PreprocessorHandler? _defaultCast;

        private object? _value;
        private string? _rawValue;
        internal bool _isEnabled = true;
        private string? _textError;
        private bool _isValid;
        internal string? _requiredText;
        internal bool _isRequired;
        internal string? _customName;
        private string _name = "undefined";
        internal bool _isGenericStringType;
        private bool unlockVisualValid;

        /// <summary>
        /// Implementation INotifyPropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Fires when this validator has encountered an error or the error 
        /// has disappeared (after entering a value or manually validating)
        /// </summary>
        public event EventHandlerSure<ValidatorErrorTextArgs>? ErrorChanged;

        /// <summary>
        /// Fires when a value has been changed 
        /// </summary>
        public event EventHandlerSure<ValidatorValueChangedArgs>? ValueChanged;

        /// <summary>
        /// Fires when a property IsEnabled has been changed
        /// </summary>
        public event EventHandlerSure<bool>? EnabledChanged;

        #region props
        /// <summary>
        /// This property is for binding and read input value
        /// </summary>
        public object? Value
        {
            get => _value;
            set
            {
                SetValue(_value, value, true, true, true);
            }
        }

        /// <summary>
        /// This property is for text based field binding only. <br/>
        /// Such as scenario:  <br/>
        /// - string: write user name <br/>
        /// - int: write number of members in the family <br/>
        /// - double: manual input of the weight of the load
        /// </summary>
        public string? RawValue
        {
            get => _rawValue ?? _value?.ToString();
            set
            {
                if (_rawValue == value)
                    return;

                if (RawCastValue(_rawValue, value, out var result, out var raw))
                {
                    _rawValue = raw;

                    if (!Equals(_value, result))
                        SetValue(_value, result, false, true, true);
                }
            }
        }

        /// <summary>
        /// The value that was specified during Building of the validator
        /// </summary>
        public object? InitValue 
        {
            get; 
            internal set; 
        }

        /// <summary>
        /// Indicates whether the validator is enabled or not. 
        /// Specify FALSE and in all checks of this validator it will return IsValid = true
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                EnabledChanged?.Invoke(this, value);

                if (!value)
                    ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Hide);
            }
        }

        /// <summary>
        /// Indicates whether the validator contains a value or not
        /// </summary>
        public bool HasValue => !CheckValueIsEmpty(Value);

        /// <summary>
        /// Indicates that any data must be entered (not null, not empty string, not white spaces)
        /// </summary>
        public bool IsRequired
        {
            get => _isRequired;
            internal set
            {
                _isRequired = value;
                OnPropertyChanged(nameof(IsRequired));
            }
        }

        /// <summary>
        /// Validation flag
        /// </summary>
        public bool IsValid
        {
            get => _isValid;
            private set
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
                OnPropertyChanged(nameof(IsVisualValid));
            }
        }

        /// <summary>
        /// Humal-like validation flag for easy XAML binding
        /// </summary>
        public bool IsVisualValid
        {
            get
            {
                if (!unlockVisualValid)
                    return true;
                else
                    return IsValid && TextError == null;
            }
        }

        /// <summary>
        /// Contains first match error
        /// </summary>
        public string? TextError
        {
            get => _textError;
            private set
            {
                _textError = value;
                OnPropertyChanged(nameof(TextError));
            }
        }

        /// <summary>
        /// The name of the validator. 
        /// If you use automatic BUILD generation, the Fody postprocessor will assign 
        /// the name of the property that the validator is bound to.
        /// Such as: 
        /// <code>
        /// public Validator{string} UserName => Validator{string}.Build()...
        /// </code>
        /// Will used UserName
        /// P.S. {} - is instead of triangle brackets, sorry
        /// </summary>
        public string Name
        {
            get => _customName ?? _name;
            internal set
            {
                _name = value;
            }
        }
        #endregion props

        public abstract bool CanNotBeNull { get; }
        protected abstract int RuleCount { get; }
        protected abstract object CreateDefaultValue();
        protected abstract ValidatorResult ExecuteRule(object? value, int ruleId);
        protected abstract void ThrowValueChangeListener(object? oldValue, object? newValue);
        protected abstract bool TryCastValue(object? value, out object? cast);
        protected abstract object? CastValue(object value);

        #region internal methods
        internal void SetValue(object? old, object? newest, bool updateRaw, bool useValidations, bool usePreprocessors)
        {
            ValidatorResult? prepError = null;
            string? newRaw = null;
            object? newValue = newest;
            bool forceUpdateRaw = false;
            if (usePreprocessors)
            {
                var args = new ValidatorPreprocessArgs
                {
                    NewValue = newValue,
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
            }

            _value = newValue;

            if (updateRaw || forceUpdateRaw)
                _rawValue = newRaw;

            if (useValidations)
            {
                ValidatorResult res;
                if (prepError != null)
                    res = prepError.Value;
                else
                    res = InternalCheckValid(_value, true, false);

                unlockVisualValid = true;
                IsValid = res.IsValid;
                TextError = res.TextError;
                ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Calc(!res.IsValid, res.TextError));
            }

            if (!Equals(old, _value))
            {
                ValueChanged?.Invoke(this, new ValidatorValueChangedArgs(old, _value));
                ThrowValueChangeListener(old, _value);
                OnPropertyChanged(nameof(Value));
            }

            if (updateRaw)
                OnPropertyChanged(nameof(RawValue));
        }

        private bool RawCastValue(string? old, string? newest, out object? result, out string? raw)
        {
            var args = new ValidatorPreprocessArgs
            {
                OldValue = old,
                NewValue = newest,
            };

            if (_defaultCast != null)
            {
                var castResult = _defaultCast(args);
                if (castResult.ResultType == PreprocessResultType.Success)
                {
                    raw = castResult.RawResult;
                    result = castResult.ValueResult;
                    return true;
                }
            }
            else
            {
                result = newest;
                raw = newest;
                return true;
            }

            result = null;
            raw = newest;
            return false;
        }

        private ValidatorResult InternalCheckValid(object? genericValue, bool useValidation, bool usePreprocessors)
        {
            bool isValid = true;
            string? textError = null;

            if (!IsEnabled)
                goto skip;

            if (IsRequired)
            {
                if (CheckValueIsEmpty(genericValue))
                {
                    isValid = false;
                    textError = _requiredText;
                    goto skip;
                }
            }

            if (usePreprocessors)
            {
                foreach (var item in _preprocess)
                {
                    var res = item(new ValidatorPreprocessArgs
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

        private bool CheckValueIsEmpty(object? genericValue)
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

        /// <summary>
        /// THIS PROPERTY IMPORTANT for Fody postprocessor
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetNameByFodyPostprocessing(string name)
        {
            _name = name;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion internal methods

        #region public methods
        /// <summary>
        /// Manually checking the validator
        /// </summary>
        public ValidatorResult CheckValid()
        {
            var res = InternalCheckValid(Value, true, true);
            unlockVisualValid = true;
            IsValid = res.IsValid;
            TextError = res.TextError;
            ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Calc(!res.IsValid, res.TextError));
            return res;
        }

        /// <summary>
        /// The rat's method of setting a value. Use it very carefully
        /// </summary>
        public void SetValueAsRat(object? value, RatModes mode)
        {
            bool skipValidations = mode.HasFlag(RatModes.SkipValidation);
            bool skipPreprocessors = mode.HasFlag(RatModes.SkipPreprocessors);

            if (mode.HasFlag(RatModes.Default))
            {
                SetValue(_value, value, true, !skipValidations, !skipPreprocessors);
            }

            if (mode.HasFlag(RatModes.InitValue))
            {
                InitValue = value;
            }
        }

        /// <summary>
        /// The rat's method for handmade setup error
        /// </summary>
        public void SetError(string textError)
        {
            IsValid = false;
            TextError = textError;
            ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Calc(true, textError));
        }

        /// <summary>
        /// Finds all validators inside the specified object using reflection
        /// </summary>
        /// <param name="validatorHoster"></param>
        public static Validator[] GetAll(object validatorHoster)
        {
            var list = new List<Validator>();
            var t = validatorHoster.GetType();
            var rootProperties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var rootProperty in rootProperties)
            {
                if (rootProperty.PropertyType.BaseType == typeof(Validator))
                {
                    var v = rootProperty.GetValue(validatorHoster);
                    if (v is Validator validator)
                        list.Add(validator);
                }
                // container
                else if (rootProperty.PropertyType
                    .GetInterfaces()
                    .Any(x => x == typeof(IValidatorHost)))
                {
                    var container = rootProperty.GetValue(validatorHoster);
                    var containerFiels = GetAll(container);
                    list.AddRange(containerFiels);
                }
                // list, array, etc
                else if (rootProperty.PropertyType
                    .GetInterfaces()
                    .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    var genericT = rootProperty.PropertyType.GenericTypeArguments.FirstOrDefault();
                    if (genericT == null)
                        continue;

                    if (genericT.GetInterfaces().Any(x => x == typeof(IValidatorHost)))
                    {
                        var v = rootProperty.GetValue(validatorHoster);
                        if (v is IEnumerable<IValidatorHost> collection)
                        {
                            foreach (var item in collection)
                            {
                                var fields = GetAll(item);
                                if (fields != null && fields.Length > 0)
                                    list.AddRange(fields);
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }
    }
    #endregion public methods
}
#nullable disable