using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidatorSam.Fody.Utils;

namespace ValidatorSam.Fody.Extensions
{
    public static  class MethodExt
    {
        public static bool IsMethodGetterValidator(this MethodDefinition methodDefinition, bool usingDebug = true)
        {
            if (methodDefinition == null)
                return false;

            if (!methodDefinition.HasBody)
                return false;

            var il = methodDefinition.Body.Instructions.First();
            if (il.OpCode == OpCodes.Call &&
                il.Operand is MethodReference methodReference)
            {
                if (usingDebug)
                    DebugFile.WriteLine($"   check is method \"{methodDefinition.Name}\" getter Validator<T> => {methodReference.FullName}");

                //if (methodReference.FullName == "ValidatorSam.ValidatorBuilder`1<!0> ValidatorSam.Validator`1<System.String>::Build()")
                //    return true;

                var returnTypeName = methodDefinition.ReturnType.FullName;
                if (usingDebug)
                    DebugFile.WriteLine($"      return type: {returnTypeName}");

                var type = returnTypeName.ExtractTextInsideAngleBrackets();
                string compareString = $"ValidatorSam.ValidatorBuilder`1<!0> ValidatorSam.Validator`1<{type}>::Build()";
                if (methodReference.FullName == compareString)
                {
                    if (usingDebug)
                        DebugFile.WriteLine($"      this is getter Validator<T>");
                    return true;
                }
                else
                {
                    if (usingDebug)
                        DebugFile.WriteLine($"      compare string \"{compareString}\" - NO EQUAL");
                }
            }

            return false;
        }
    }
}
