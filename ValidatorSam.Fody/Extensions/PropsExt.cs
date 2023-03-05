using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam.Fody.Extensions
{
    internal static class PropsExt
    {
        public static bool IsAutoValidator(this PropertyDefinition prop, bool usingDebug = true)
        {
            if (prop.SetMethod != null)
                return false;

            if (prop.GetMethod.IsMethodGetterValidator(usingDebug))
                return true;

            return false;
        }
    }
}
