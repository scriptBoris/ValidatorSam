using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam.Core
{
#nullable enable
    public interface IValidatorBroadcaster
    {
        /// <summary>
        /// Fires when this validator has encountered an error or the error 
        /// has disappeared (after entering a value or manually validating)
        /// </summary>
        event EventHandlerSure<ValidatorErrorTextArgs>? ErrorChanged;

        /// <summary>
        /// Fires when a property IsEnabled has been changed
        /// </summary>
        event EventHandlerSure<bool>? EnabledChanged;
    }
#nullable disable
}
