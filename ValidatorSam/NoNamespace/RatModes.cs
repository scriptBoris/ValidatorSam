using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam
{
    [Flags]
    public enum RatModes
    {
        /// <summary>
        /// Value
        /// </summary>
        Default = 0,

        /// <summary>
        /// Init value
        /// </summary>
        InitValue = 1,

        /// <summary>
        /// Without calculate rules and limitations
        /// </summary>
        SkipValidation = 2,

        /// <summary>
        /// Without calculate preprocessors
        /// </summary>
        SkipPreprocessors = 4,
    }
}
