using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam.Core
{
#nullable enable
    /// <summary>
    /// Container for external rule result
    /// </summary>
    public struct ExternalRuleResult
    {
        /// <summary>
        /// External rule no contains errors
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Text of error if external rule is invalid
        /// </summary>
        public string ErrorText { get; set; }

        /// <summary>
        /// Success instance
        /// </summary>
        public static ExternalRuleResult Success
        {
            get 
            {
                return new ExternalRuleResult
                {
                    IsSuccess = true,
                    ErrorText = string.Empty,
                };
            }
        }

        /// <summary>
        /// Make invalid instance
        /// </summary>
        public static ExternalRuleResult Error(string textError)
        {
            return new ExternalRuleResult
            {
                IsSuccess = false,
                ErrorText = textError,
            };
        }
    }
#nullable disable
}