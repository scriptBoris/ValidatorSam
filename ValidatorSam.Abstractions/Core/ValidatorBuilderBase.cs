using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam.Core
{
    public abstract class ValidatorBuilderBase<T>
    {
        public const string defaultRequired = "{DEFAULT}";

        /// <summary>
        /// Creates a rule that will perform a validation check every time 
        /// a Value or RawValue is changed or a manual check is performed.
        /// <br/>
        /// ---
        /// <br/>
        /// Return <c>false</c> for mark this Validator property IsValid as false
        /// </summary>
        /// <param name="rule">function that will be called when a new value is received. If false is returned, an error will be set</param>
        /// <param name="error">the message that will be displayed if the rule function returns false</param>
        public abstract ValidatorBuilderBase<T> UsingRule(RuleHandler<T> rule, string error);

        /// <inheritdoc cref="UsingRule(RuleHandler{T}, string)"/>
        /// <param name="rule"></param>
        /// <param name="getError">dynamic function to get error text</param>
        public abstract ValidatorBuilderBase<T> UsingRule(RuleHandler<T> rule, Func<string> getError);

        /// <summary>
        /// Sets the <see cref="IValidator.IsRequired"/> property to <c>true</c>, indicating that 
        /// <see cref="IValidator.Value"/> must not be empty. If this validator contains an empty 
        /// value, the <see cref="IValidator.IsValid"/> property will be set to <c>false</c>.
        /// <br/><br/>
        /// <b>Empty values are determined based on the following rules:</b>
        /// </summary>
        /// <remarks>
        /// <para><b>Strings:</b> A value is considered empty if it is <c>null</c>, an 
        /// empty string (<c>""</c>), or consists only of whitespace.</para>
        ///
        /// <para><b>Numbers:</b> Since numeric types (e.g., <see cref="int"/>, 
        /// <see cref="long"/>) are structs and cannot be <c>null</c>, the validator 
        /// determines an empty state using the <see cref="IValidator.RawValue"/> property. If 
        /// <see cref="IValidator.RawValue"/> is <c>null</c>, an empty string, or a string 
        /// containing only whitespace, it is considered empty.</para>
        ///
        /// <para><b>Other types:</b> A value is considered empty if it is <c>null</c>.</para>
        /// </remarks>
        /// <param name="requiredText">
        /// Custom error message. If omitted, a default error message is automatically 
        /// assigned based on the <see cref="System.Threading.Thread.CurrentCulture"/> setting.
        /// </param>
        public abstract ValidatorBuilderBase<T> UsingRequired(string requiredText = defaultRequired);

        /// <inheritdoc cref="UsingRequired(string)"/>
        /// <param name="getError">dynamic function to get error text</param>
        public abstract ValidatorBuilderBase<T> UsingRequired(Func<string> getError);

        /// <summary>
        /// Sets the property Name. If not using his method the system automatically chooses name
        /// based by the name of this Validator property
        /// <br/>
        /// ---
        /// <br/>
        /// tip: useful if you want to map the name of an input field UI to an error token from the 
        /// server, for example
        /// </summary>
        /// <param name="name">Name</param>
        public abstract ValidatorBuilderBase<T> UsingName(string name);

        /// <summary>
        /// Sets the initial value
        /// </summary>
        /// <param name="value">initial value</param>
        public abstract ValidatorBuilderBase<T> UsingValue(T value);

        /// <summary>
        /// Sets the IsEnabled property
        /// <br/>
        /// ---
        /// <br/>
        /// For more info see <see cref="IValidator.IsEnabled"/>
        /// </summary>
        /// <param name="isEnabled">enabled flag</param>
        public abstract ValidatorBuilderBase<T> UsingEnabledState(bool isEnabled);

        /// <summary>
        /// Creates a preprocessor that may will modificated Value before it set
        /// <br/>
        /// ---
        /// <br/>
        /// tip: use it if you want to create a phone mask or something like that.
        /// </summary>
        /// <param name="cast">preprocessor</param>
        public abstract ValidatorBuilderBase<T> UsingPreprocessor(PreprocessorHandler<T> cast);

        /// <summary>
        /// Easy to way get callback action when value was change
        /// </summary>
        /// <param name="act">Event args</param>
        public abstract ValidatorBuilderBase<T> UsingValueChangeListener(Action<ValidatorValueChangedArgs<T>> act);

        /// <summary>
        /// Payload
        /// </summary>
        public abstract ValidatorBuilderBase<T> UsingPayload(string key, object payload);

        /// <summary>
        /// Using converter
        /// </summary>
        public abstract ValidatorBuilderBase<T> UsingConverter(IValueRawConverter<T> converter);
    }
}
#nullable disable