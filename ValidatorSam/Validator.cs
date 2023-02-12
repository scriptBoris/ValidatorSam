using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using ValidatorSam.Core;

#nullable enable
namespace ValidatorSam
{
    public abstract partial class Validator : INotifyPropertyChanged
    {
        protected readonly List<Func<ValidatorPreprocessArgs, PreprocessResult>> _preprocess =
            new List<Func<ValidatorPreprocessArgs, PreprocessResult>>();

        private object? _value;
        private string? _rawValue;
        private bool _isEnabled = true;
        private string? _textError;
        private bool _isValid;
        protected string? _requiredText;
        private bool _isRequired;
        private string? _name;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<ValidatorErrorTextArgs>? ErrorChanged;
        public event EventHandler<ValidatorValueChangedArgs>? ValueChanged;
        public event EventHandler<bool>? EnabledChanged;

        #region props
        public object? Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                var castResult = CastValue(_value, value);
                _value = castResult.Result;
                _rawValue = castResult.TextResult;

                var res = InternalCheckValid(_value);
                IsValid = res.IsValid;
                TextError = res.TextError;
                ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Calc(!res.IsValid, res.TextError));
                ValueChanged?.Invoke(this, new ValidatorValueChangedArgs(oldValue, _value));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(RawValue));
            }
        }

        public string? RawValue
        {
            get => _rawValue ?? _value?.ToString();
            set => Value = value;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            private set
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
            private set => _name = value;
        }
        #endregion props

        protected abstract int RuleCount { get; }
        protected abstract bool CanNotBeNull { get; }
        protected abstract object CreateDefaultValue();
        protected abstract ValidatorResult ExecuteRule(object? value, int ruleId);
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

            var final = newestT;
            var args = new ValidatorPreprocessArgs
            {
                IsString = newStr != null,
                StringOldValue = oldStr,
                StringNewValue = newStr,
                OldValue = oldT,
                NewValue = newestT,
            };

            PreprocessResult? triggerPreprocessor = null;
            foreach (var item in _preprocess)
            {
                var preprocessResult = item.Invoke(args);
                if (preprocessResult.IsSkip)
                    continue;

                triggerPreprocessor = preprocessResult;
                final = preprocessResult.Result;
                break;
            }

            // catch value for elementary type (int, 
            if (final == null && CanNotBeNull)
                final = CreateDefaultValue();

            return new PreprocessResult
            {
                TextResult = triggerPreprocessor?.TextResult ?? final?.ToString(),
                Result = final,
            };
        }

        private ValidatorResult InternalCheckValid(object? genericValue)
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

        protected void SetValueAsRat(object value)
        {
            _value = value;
            _rawValue = value?.ToString();
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

            var res = InternalCheckValid(Value);
            IsValid = res.IsValid;
            TextError = res.TextError;
            return res;
        }
        #endregion public methods
    }
}
#nullable disable