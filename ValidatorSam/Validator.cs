using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ValidatorSam.Core;
using ValidatorSam.Internal;
using System.Globalization;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Base class validator
    /// </summary>
    public abstract class Validator : IValidator, INotifyPropertyChanged, IEventBroadcaster
    {
        internal ISourceRequired? _required;
        internal bool _isEnabled = true;
        internal bool _isGenericStringType;
        internal string? _customName;
        internal Payload? _payload;
        internal ExternalRuleHandler? _externalRule;
        private string? _textError;
        private bool _isValid;
        private string _name = "undefined";
        private readonly object _lock = new object();

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public event EventHandlerSure<ValidatorErrorTextArgs>? ErrorChanged;

        /// <inheritdoc/>
        public event EventHandlerSure<ValidatorValueChangedArgs>? ValueChanged;

        /// <inheritdoc/>
        public event EventHandlerSure<bool>? EnabledChanged;

        /// <inheritdoc/>
        public event EventHandlerSure<bool>? ValidationChanged;

        #region props
        /// <inheritdoc/>
        public object? Value
        {
            get => GenericGetValue();
            set => GenericSetValue(value);
        }

        /// <inheritdoc/>
        public abstract string RawValue { get; set; }
        
        /// <inheritdoc/>
        public object? InitValue 
        {
            get; 
            internal set; 
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IPayload Payload 
        {
            get
            {
                if (_payload == null)
                    lock(_lock)
                    {
                        _payload ??= new Payload(this);
                    }
                return _payload;
            }
        }

        /// <inheritdoc/>
        public abstract string? StringFormat { get; set; }

        /// <inheritdoc/>
        public abstract CultureInfo? CultureInfo { get; set; }

        /// <inheritdoc/>
        public abstract bool HasValue { get; }

        /// <inheritdoc/>
        public bool IsRequired => _required != null;

        /// <inheritdoc/>
        public bool IsValid
        {
            get => _isValid;
            protected set
            {
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        /// <inheritdoc/>
        public abstract bool IsVisualValid { get; }

        /// <inheritdoc/>
        public string? TextError
        {
            get => _textError;
            protected set
            {
                _textError = value;
                OnPropertyChanged(nameof(TextError));
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get => _customName ?? _name;
            internal set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Indicate that type T is non nullable struct type
        /// </summary>
        public abstract bool CanNotBeNull { get; }

        /// <summary>
        /// Internal
        /// </summary>
        protected abstract int RuleCount { get; }
        #endregion props

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
        /// <param name="useValidations">Включить правила валидации</param>
        /// <param name="usePreprocessors"></param>
        internal abstract void SetValue(object? old, object? newest, ValueInvokers invoker, bool useValidations, bool usePreprocessors);

        #region internal methods
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
        public abstract ValidatorResult CheckValid();

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
                SetValue(_value, value, ValueInvokers.Value, !skipValidations, !skipPreprocessors);
            }
        }

        /// <summary>
        /// The rat's method for handmade setup error
        /// </summary>
        public abstract void SetError(string textError);

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
        /// Adding an external rule that will be taken into account during validation 
        /// with the highest priority
        /// </summary>
        public void SetExternalRule(ExternalRuleHandler? externalRuleDelegate)
        {
            _externalRule = externalRuleDelegate;
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