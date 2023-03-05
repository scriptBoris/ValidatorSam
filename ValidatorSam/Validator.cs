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
        private string _name;
        internal bool _isGenericStringType;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandlerSure<ValidatorErrorTextArgs>? ErrorChanged;
        public event EventHandlerSure<ValidatorValueChangedArgs>? ValueChanged;
        public event EventHandlerSure<bool>? EnabledChanged;

        #region props
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

        public string? RawValue
        {
            get => _rawValue ?? _value?.ToString();
            set => Value = value;
        }

        public object? InitValue 
        {
            get; 
            internal set; 
        }

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

        public bool HasValue => !CheckValueIsEmpty(Value);

        public bool IsRequired
        {
            get => _isRequired;
            internal set
            {
                _isRequired = value;
                OnPropertyChanged(nameof(IsRequired));
            }
        }

        public bool IsValid
        {
            get => _isValid;
            private set
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public string? TextError
        {
            get => _textError;
            private set
            {
                _textError = value;
                OnPropertyChanged(nameof(TextError));
            }
        }

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

            if (genericValue is string vstring)
            {
                isEmpty = string.IsNullOrWhiteSpace(vstring);
            }
            //else if (IsGenericStructType())
            //{
            //    isEmpty = value == null;
            //}
            else
            {
                isEmpty = genericValue == null;
            }

            return isEmpty;
        }

        internal void SetValueAsRat(object? value, bool asInitValue)
        {
            _value = value;
            _rawValue = value?.ToString();

            if (asInitValue)
            {
                InitValue = value;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion internal methods

        #region public methods
        public ValidatorResult CheckValid()
        {
            //var value = CastValue(Value);

            var res = InternalCheckValid(Value, new PreprocessResult());
            IsValid = res.IsValid;
            TextError = res.TextError;
            ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Calc(!res.IsValid, res.TextError));
            return res;
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
    #endregion public methods
}
#nullable disable