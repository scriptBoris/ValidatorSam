using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        private static readonly Dictionary<string, ValidatorLocalization> _cache = new Dictionary<string, ValidatorLocalization>();
        private static ValidatorLocalization? ResolvedLocalization;

        /// <summary>
        /// Exclicit culture info
        /// </summary>
        public static CultureInfo? CultureInfo { get; set; }

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
                
                var currentCulture = CultureInfo ?? CultureInfo.CurrentCulture;

                if (_cache.TryGetValue(currentCulture.TwoLetterISOLanguageName, out var match))
                {
                    ResolvedLocalization = match;
                }
                else
                {
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
                    _cache.Add(currentCulture.TwoLetterISOLanguageName, ResolvedLocalization);
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

        /// <summary>
        /// Please enter the date in the format
        /// </summary>
        public abstract string StringInvalidInputForDateTime { get; }

        /// <summary>
        /// The entered month cannot contain more than {0} days
        /// </summary>
        public abstract string StringMonthIsOverflow { get; }

        /// <summary>
        /// Invalid year
        /// </summary>
        public abstract string StringInvalidYear { get; }

        /// <summary>
        /// Invalid month
        /// </summary>
        public abstract string StringInvalidMonth { get; }

        /// <summary>
        /// Invalid day
        /// </summary>
        public abstract string StringInvalidDay { get; }

        /// <summary>
        /// Invalid hour
        /// </summary>
        public abstract string StringInvalidHour { get; }

        /// <summary>
        /// Invalid minute
        /// </summary>
        public abstract string StringInvalidMinute { get; }

        /// <summary>
        /// Invalid second
        /// </summary>
        public abstract string StringInvalidSecond { get; }
    }
}
#nullable disable