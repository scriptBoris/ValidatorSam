using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;

namespace ValidatorSam.Fody.Extensions
{
    internal static class PropsExt
    {
        public static bool IsAutoValidator(this PropertyDefinition prop)
        {
            if (prop.SetMethod != null)
                return false;

            if (prop.GetMethod.IsMethodAutoGen())
                return true;

            return false;
        }
    }
}
