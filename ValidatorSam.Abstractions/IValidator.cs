using System;
using System.ComponentModel;
using System.Globalization;

#nullable enable
namespace ValidatorSam
{
    public interface IValidator
    {
        /// <summary>
        /// Implementation INotifyPropertyChanged event
        /// </summary>
        event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Fires when this validator has encountered an error or the error 
        /// has disappeared (after entering a value or manually validating)
        /// </summary>
        event EventHandlerSure<ValidatorErrorTextArgs>? ErrorChanged;

        /// <summary>
        /// Fires when a value has been changed 
        /// </summary>
        event EventHandlerSure<ValidatorValueChangedArgs>? ValueChanged;

        /// <summary>
        /// Fires when a property IsEnabled has been changed
        /// </summary>
        event EventHandlerSure<bool>? EnabledChanged;

        /// <summary>
        /// Fires when a property IsValid has been changed
        /// </summary>
        event EventHandlerSure<bool>? ValidationChanged;

        /// <summary>
        /// The value that was specified during Building of the validator
        /// </summary>
        object? InitValue { get; }

        /// <summary>
        /// Indicates whether the validator is enabled or not. 
        /// Specify FALSE and in all checks of this validator it will return IsValid = true
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// This property is for text based field binding only. <br/>
        /// Such as scenario:  <br/>
        /// - string: write user name <br/>
        /// - int: write number of members in the family <br/>
        /// - double: manual input of the weight of the load
        /// </summary>
        string RawValue { get; set; }

        /// <summary>
        /// This property is for binding and read input value
        /// </summary>
        object? Value { get; set; }

        /// <summary>
        /// Validation flag 
        /// <br/>
        /// By default this property contains False;
        /// It will be changed if Value, RawValue is changed or manual validation is called
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Humal-like validation flag for easy XAML binding.
        /// <br/>
        /// If <c>Value</c> or <c>RawValue</c> is changed or manual validation is called, this 
        /// property will return IsValid;
        /// Otherwise: this property will return True;
        /// </summary>
        bool IsVisualValid { get; }

        /// <summary>
        /// Contains first match error
        /// </summary>
        string? TextError { get; }

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
        string Name { get; }

        /// <summary>
        /// Payload that can be used for different purposes
        /// </summary>
        IPayload Payload { get; }

        /// <summary>
        /// Special string for formating Value to RawValue, and reverse;
        /// <br/>
        /// If it contains null, then converters can interpret the format themselves. 
        /// </summary>
        string? StringFormat { get; }

        /// <summary>
        /// Special string for formating Value to RawValue, and reverse;
        /// <br/>
        /// If it contains null, then converters can interpret the format themselves. 
        /// </summary>
        CultureInfo? CultureInfo { get; }

        /// <summary>
        /// Indicates whether the validator contains a value or not
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Indicates that any data must be entered (not null, not empty string, not white spaces)
        /// </summary>
        bool IsRequired { get; }
    }
}
#nullable disable