using System;
using ValidatorSam.Internal;
using ValidatorSam.Converters;
using ValidatorSam.Core;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Class for initialization Validator
    /// </summary>
    public class ValidatorBuilder<T> : ValidatorBuilderBase<T>
    {
        internal readonly Validator<T> Validator = new Validator<T>();

        /// <summary>
        /// Internal ctor.
        /// For using Builder with non-Fody way, use <see cref="Validator{T}.BuildManual"/>
        /// </summary>
        internal ValidatorBuilder()
        {
        }

        internal static ValidatorBuilder<T> Build(string propName)
        {
            var self = new ValidatorBuilder<T>();
            self.Validator.Name = propName;
            return self;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingRule(RuleHandler<T> rule, string error)
        {
            Validator._rules.Add(new RuleItem<T>(error, rule));
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingRule(RuleHandler<T> rule, Func<string> getError)
        {
            Validator._rules.Add(new DynamicRuleItem<T>(getError, rule));
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingRequired(string requiredText = defaultRequired)
        {
            Validator._required = new StaticRequired(requiredText);
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingRequired(Func<string> getError)
        {
            Validator._required = new DynamicRequired(getError);
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingName(string name)
        {
            Validator._customName = name;
            return this;
        }


        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingValue(T value)
        {
            Validator.InitValue = value;
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingEnabledState(bool isEnabled)
        {
            Validator._isEnabled = isEnabled;
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingPreprocessor(PreprocessorHandler<T> cast)
        {
            Validator._preprocess.Add(cast);
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingValueChangeListener(Action<ValidatorValueChangedArgs<T>> act)
        {
            Validator._changeListeners.Add(act);
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingPayload(string key, object payload)
        {
            Validator.Payload.Push(key, payload);
            return this;
        }

        /// <inheritdoc/>
        public override ValidatorBuilderBase<T> UsingConverter(IValueRawConverter<T> converter)
        {
            Validator._defaultCastConverter = converter;
            return this;
        }

        internal void ResolveAutoCast(Type type)
        {
            var nullableGenericType = Nullable.GetUnderlyingType(type);
            if (nullableGenericType != null)
                type = nullableGenericType;

            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    Validator._defaultCastConverter = new NumbersConverter<T>(typeCode);
                    break;
                case TypeCode.DateTime:
                    {
                        // default DateTime
                        if (nullableGenericType == null)
                        {
                            var dtConverter = new DateTimeConverter(Validator.StringFormat);
                            if (dtConverter is IValueRawConverter<T> dtConverterT)
                                Validator._defaultCastConverter = dtConverterT;
                        }
                        // nullable DateTime
                        else
                        {
                            var dtNullConverter = new DateTimeNullConverter(Validator.StringFormat);
                            if (dtNullConverter is IValueRawConverter<T> dtNullConverterT)
                                Validator._defaultCastConverter = dtNullConverterT;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
#nullable disable