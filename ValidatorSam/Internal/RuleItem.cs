using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam.Internal
{
    internal class RuleItem<T>
    {
        public Func<T, bool> Delegate { get; set; }
        public string ErrorText { get; set; }
        public bool IsSafeRule { get; set; }
    }
}
