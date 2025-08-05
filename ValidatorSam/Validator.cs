using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ValidatorSam.Core;
using ValidatorSam.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Default delegate validator
    /// </summary>
    public delegate void EventHandlerSure<T>(Validator invoker, T args);

    /// <summary>
    /// Default delegate validator
    /// </summary>
    public delegate void EventHandlerSure(Validator invoker);

    /// <summary>
    /// Delegate for preprocess input value
    /// </summary>
    public delegate PreprocessResult<T> PreprocessorHandler<T>(ValidatorPreprocessArgs<T> args);

    /// <summary>
    /// Base class validator
    /// </summary>
    public abstract class Validator : INotifyPropertyChanged
    {
        internal ISourceRequired? _required;

        internal bool _isEnabled = true;
        private string? _textError;
        private bool _isValid;
        internal string? _customName;
        private string _name = "undefined";
        internal bool _isGenericStringType;
        internal CultureInfo? _cultureInfo;
        protected bool unlockVisualValid;

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

        /// <summary>
        /// Fires when a property IsValid has been changed
        /// </summary>
        public event EventHandlerSure<bool>? ValidationChanged;

        #region props
        /// <summary>
        /// This property is for binding and read input value
        /// </summary>
        public object? Value
        {
            get => GenericGetValue();
            set => GenericSetValue(value);
        }

        /// <summary>
        /// This property is for text based field binding only. <br/>
        /// Such as scenario:  <br/>
        /// - string: write user name <br/>
        /// - int: write number of members in the family <br/>
        /// - double: manual input of the weight of the load
        /// </summary>
        public abstract string? RawValue { get; set; }
        
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
        /// Special string for formating Value to RawValue, and reverse;
        /// <br/>
        /// If contains are null
        /// </summary>
        public abstract string? StringFormat { get; set; }

        /// <summary>
        /// Indicates whether the validator contains a value or not
        /// </summary>
        public abstract bool HasValue { get; }

        /// <summary>
        /// Indicates that any data must be entered (not null, not empty string, not white spaces)
        /// </summary>
        public bool IsRequired => _required != null;

        /// <summary>
        /// Validation flag 
        /// <br/>
        /// By default this property contains False;
        /// It will be changed if Value, RawValue is changed or manual validation is called
        /// </summary>
        public bool IsValid
        {
            get => _isValid;
            protected set
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        /// <summary>
        /// Humal-like validation flag for easy XAML binding.
        /// <br/>
        /// If <c>Value</c> or <c>RawValue</c> is changed or manual validation is called, this 
        /// property will return IsValid;
        /// Otherwise: this property will return True;
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
            protected set
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

        /// <summary>
        /// Indicate that type T is non nullable struct type
        /// </summary>
        public abstract bool CanNotBeNull { get; }

        /// <summary>
        /// Internal
        /// </summary>
        protected abstract int RuleCount { get; }

        /// <summary>
        /// Получает _value;
        /// </summary>
        protected abstract object? GenericGetValue();

        /// <summary>
        /// Устанавливает _value как SetValue(...); для правильной работы Value
        /// </summary>
        protected abstract void GenericSetValue(object? value);

        /// <summary>
        /// Устанавливает значение для Value
        /// </summary>
        /// <param name="old">Старое значение</param>
        /// <param name="newest">Новое значение</param>
        /// <param name="invoker">Источник вызова этого метода</param>
        /// <param name="updateRaw">Обновлять ли RawValue</param>
        /// <param name="useValidations">Включить правила валидации</param>
        /// <param name="usePreprocessors"></param>
        internal abstract void SetValue(object? old, object? newest, ValueInvokers invoker, bool updateRaw, bool useValidations, bool usePreprocessors);

        #region internal methods
        internal abstract ValidatorResult InternalCheckValid(object? genericValue, bool useValidation, bool usePreprocessors);

        /// <summary>
        /// THIS PROPERTY IMPORTANT for Fody postprocessor
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetNameByFodyPostprocessing(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Implementation of IPropertyChanged
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Invoke event
        /// </summary>
        protected void InvokeValidationChanged(Validator invoker, bool isValid)
        {
            ValidationChanged?.Invoke(invoker, IsValid);
        }

        /// <summary>
        /// Invoke event
        /// </summary>
        protected void InvokeErrorChanged(Validator invoker, ValidatorErrorTextArgs args)
        {
            ErrorChanged?.Invoke(invoker, args);
        }

        /// <summary>
        /// Invoke event
        /// </summary>
        protected void InvokeValueChanged(Validator invoker, ValidatorValueChangedArgs args)
        {
            ValueChanged?.Invoke(invoker, args);
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
        /// If there is a visual error, then a manual validation check will be called. 
        /// And if the validation is successful (no errors), then error will be removed.
        /// </summary>
        public bool TryToRemoveError()
        {
            if (IsVisualValid)
                return false;

            var res = CheckValid();
            return res.IsValid;
        }

        /// <summary>
        /// The rat's method of setting a value. Use it very carefully
        /// </summary>
        public void SetValueAsRat(object? value, RatModes mode)
        {
            bool skipValidations = mode.HasFlag(RatModes.SkipValidation);
            bool skipPreprocessors = mode.HasFlag(RatModes.SkipPreprocessors);

            if (mode.HasFlag(RatModes.InitValue))
            {
                InitValue = value;
            }

            if (mode.HasFlag(RatModes.Default))
            {
                var _value = GenericGetValue();
                SetValue(_value, value, ValueInvokers.Value, true, !skipValidations, !skipPreprocessors);
            }
        }

        /// <summary>
        /// The rat's method for handmade setup error
        /// </summary>
        public void SetError(string textError)
        {
            unlockVisualValid = true;
            IsValid = false;
            TextError = textError;
            ErrorChanged?.Invoke(this, ValidatorErrorTextArgs.Calc(true, textError));
            OnPropertyChanged(nameof(IsVisualValid));
        }

        /// <summary>
        /// Checks if the current value is different from the initialized value
        /// </summary>
        public bool CheckChanges()
        {
            if (InitValue == null && Value == null)
            {
                return false;
            }
            else if (InitValue != null && Value == null)
            {
                return true;
            }
            else if (InitValue == null && Value != null)
            {
                return true;
            }
            else
            {
                bool isEquals = InitValue!.Equals(Value);
                return !isEquals;
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