using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ValidatorSam.Localizations;

#nullable enable
namespace ValidatorSam.Core
{
    /// <summary>
    /// Base class localization
    /// <br/>
    /// Implement this class and set the instance to the 
    /// <see cref="ValidatorSam.Core.ValidatorLocalization.UseLocalization"/> 
    /// property to add support for languages ​​that are not supported by default by this library. 
    /// </summary>
    public abstract class ValidatorLocalization
    {
        private static ValidatorLocalization? ResolvedLocalization;

        /// <summary>
        /// Set this property for using explicit localization texts
        /// </summary>
        public static ValidatorLocalization? UseLocalization { get; set; }

        internal static ValidatorLocalization Resolve
        {
            get
            {
                if (UseLocalization != null)
                    return UseLocalization;

                if (ResolvedLocalization == null)
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    switch (currentCulture.TwoLetterISOLanguageName)
                    {
                        case "ru":
                            ResolvedLocalization = new Localization_RU();
                            break;
                        case "en":
                        default:
                            ResolvedLocalization = new Localization_EN();
                            break;
                    }
                }
                return ResolvedLocalization;

            } 
        }

        /// <summary>
        /// The value cannot be less than {0}
        /// </summary>
        public abstract string StringValueLess { get; }

        /// <summary>
        /// The value cannot be greater than {0}
        /// </summary>
        public abstract string StringValueOver { get; }

        /// <summary>
        /// The value cannot be shorter than {0} characters
        /// </summary>
        public abstract string StringLengthLess { get; }

        /// <summary>
        /// The value cannot be longer than {0} characters
        /// </summary>
        public abstract string StringLengthOver { get; }

        /// <summary>
        /// Required
        /// </summary>
        public abstract string StringRequired { get; }
    }
}
#nullable disable