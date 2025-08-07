using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam
{
    /// <summary>
    /// Container for holding validation error data 
    /// </summary>
    public struct ValidatorErrorTextArgs
    {
        /// <summary>
        /// Flag to show or hide display error
        /// </summary>
        public bool IsShow { get; private set; }

        /// <summary>
        /// Error text
        /// </summary>
        public string ErrorText { get; private set; }

        internal static ValidatorErrorTextArgs Hide => new ValidatorErrorTextArgs { IsShow = false, ErrorText = null };
        internal static ValidatorErrorTextArgs Calc(bool isShow, string text)
        {
            if (isShow)
            {
                return new ValidatorErrorTextArgs { IsShow = true, ErrorText = text };
            }
            else
            {
                return Hide;
            }
        }
    }
}
