using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Validation check result
    /// </summary>
    public readonly struct ValidatorResult
    {
        internal ValidatorResult(bool isSuccess, string? errorText, string name)
        {
            IsValid = isSuccess;
            TextError = errorText;
            Name = name;
        }

        /// <summary>
        /// Text error. Does not contain null if IsValid is false
        /// </summary>
        public string? TextError { get; }

        /// <summary>
        /// An indicator indicating that no errors were found
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// The name of the validator that is invalid. 
        /// <br/>
        /// If no errors are found (<see cref="IsValid"/> is true), then 
        /// <see cref="Name"/> will be "none"
        /// </summary>
        public string Name { get; }
    }
}
#nullable disable