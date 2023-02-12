using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam.Core
{
    public struct PreprocessResult
    {
        public bool IsSkip { get; set; }
        public string ErrorText { get; set; }
        public object Result { get; set; }
        public string TextResult { get; set; }
    }
}
