using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam
{
    public struct ValidatorErrorTextArgs
    {
        public bool IsShow { get; private set; }
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
