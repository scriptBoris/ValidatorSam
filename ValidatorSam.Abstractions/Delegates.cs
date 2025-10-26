using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace ValidatorSam
{
    /// <summary>
    /// Default delegate validator
    /// </summary>
    public delegate void EventHandlerSure<T>(IValidator invoker, T args);

    /// <summary>
    /// Default delegate validator
    /// </summary>
    public delegate void EventHandlerSure(IValidator invoker);

    /// <summary>
    /// Delegate for preprocess input value
    /// </summary>
    public delegate PreprocessResult<T> PreprocessorHandler<T>(ValidatorPreprocessArgs<T> args);

    /// <summary>
    /// Delegate for rules
    /// </summary>
    public delegate bool RuleHandler<T>(RuleArgs<T> args);

    /// <summary>
    /// Delegate for external rule
    /// </summary>
    public delegate ExternalRuleResult ExternalRuleHandler(RuleArgs<object?> args);

    /// <summary>
    /// Delegate
    /// </summary>
    public delegate void PayloadHandlerAdded(IValidator validator, IPayload payload, string key, object added);

    /// <summary>
    /// Delegate
    /// </summary>
    public delegate void PayloadHandlerRemoved(IValidator validator, IPayload payload, string key, object removed);

}
#nullable disable