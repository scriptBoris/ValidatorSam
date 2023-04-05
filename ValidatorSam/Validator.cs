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
    /// Base class validator
    /// </summary>
    public abstract class Validator : INotifyPropertyChanged
    {
        internal readonly List<Func<ValidatorPreprocessArgs, PreprocessResult>> _preprocess =
            new List<Func<ValidatorPreprocessArgs, PreprocessResult>>();

        private object? _value;
        private string? _rawValue;
        internal bool _isEnabled = true;
        private string? _textError;
        private bool _isValid;
        internal string? _requiredText;
        internal bool _isRequired;
        private string _name = "undefined";
        internal bool _isGenericStringType;

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
                var oldValue = _value;
                var castResult = CastValue(_value, value);
                _value = castResult.ValueResult;
                _rawValue = castResult.TextResult;

                var res = InternalCheckValid(_value, castResult);

                IsValid = res.IsValid;
                TextError = res.TextError;
                ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Calc(!res.IsValid, res.TextError));
                ValueChanged?.Invoke(this, new ValidatorValueChangedArgs(oldValue, _value));
                ThrowValueChangeListener(oldValue, _value);
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(RawValue));
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
            set => Value = value;
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
            get => _name; /*?? new StackTrace().GetFrame(1).Get;*/
            internal set => _name = value;
        }
        #endregion props

        protected abstract int RuleCount { get; }
        protected abstract bool CanNotBeNull { get; }
        protected abstract object CreateDefaultValue();
        protected abstract ValidatorResult ExecuteRule(object? value, int ruleId);
        protected abstract void ThrowValueChangeListener(object? oldValue, object? newValue);
        protected abstract bool TryCastValue(object? value, out object? cast);
        protected abstract object? CastValue(object value);

        #region internal methods
        private PreprocessResult CastValue(object? old, object? newest)
        {
            string? oldStr = null;
            string? newStr = null;

            if (!TryCastValue(old, out object? oldT))
            {
                if (old is string oldAsString)
                    oldStr = oldAsString;
            }

            if (!TryCastValue(newest, out object? newestT))
            {
                if (newest is string newestAsString)
                    newStr = newestAsString;
            }

            if (_isGenericStringType)
            {
                oldStr = old as string;
                newStr = newest as string;
            }

            var final = newestT;
            var args = new ValidatorPreprocessArgs
            {
                IsString = newStr != null,
                StringOldValue = oldStr,
                StringNewValue = newStr,
                OldValue = oldT,
                NewValue = newestT,
            };

            PreprocessResult? completedPreprocessor = null;
            foreach (var item in _preprocess)
            {
                var preprocessResult = item.Invoke(args);
                if (preprocessResult.Type == PreprocessTypeResult.Ignore)
                    continue;

                completedPreprocessor = preprocessResult;
                final = preprocessResult.ValueResult;

                if (preprocessResult.Type == PreprocessTypeResult.Error)
                    break;
                else
                    args = preprocessResult.AsArg(args);
            }

            // catch value for elementary type (int, 
            if (final == null && CanNotBeNull)
                final = CreateDefaultValue();

            return new PreprocessResult
            {
                Type = completedPreprocessor?.Type ?? PreprocessTypeResult.Success,
                ErrorText = completedPreprocessor?.ErrorText,
                TextResult = completedPreprocessor?.TextResult ?? final?.ToString(),
                ValueResult = final,
            };
        }

        private ValidatorResult InternalCheckValid(object? genericValue, PreprocessResult castResult)
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

            if (castResult.ErrorText != null)
            {
                return new ValidatorResult(false, castResult.ErrorText, _name);
            }

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
            //var value = CastValue(Value);

            var res = InternalCheckValid(Value, new PreprocessResult());
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
            switch (mode)
            {
                case RatModes.Init:
                    _value = value;
                    _rawValue = value?.ToString();
                    InitValue = value;
                    break;

                case RatModes.NoThrowEvents:
                    _value = value;
                    _rawValue = value?.ToString();
                    break;

                default:
                    Value = value;
                    break;
            }
        }

        /// <summary>
        /// Finds all validators inside the specified object using reflection
        /// </summary>
        /// <param name="validatorHoster"></param>
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
    #endregion public methods
}
#nullable disable