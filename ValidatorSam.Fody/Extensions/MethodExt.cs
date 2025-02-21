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
        public static bool IsMethodGetterValidator(this MethodDefinition methodDefinition, bool usingDebug = true, int consoleSpacing = 0)
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
                {
                    DebugFile.WriteLine($"check is method <{methodDefinition.Name}> getter Validator<T> => {methodReference.FullName}", consoleSpacing);
                    consoleSpacing++;
                }


                //if (methodReference.FullName == "ValidatorSam.ValidatorBuilder`1<!0> ValidatorSam.Validator`1<System.String>::Build()")
                //    return true;

                var returnTypeName = methodDefinition.ReturnType.FullName;
                if (usingDebug)
                    DebugFile.WriteLine($"return type: {returnTypeName}", consoleSpacing);

                var type = returnTypeName.ExtractTextInsideAngleBrackets();
                string compareString = $"ValidatorSam.ValidatorBuilder`1<!0> ValidatorSam.Validator`1<{type}>::Build()";
                if (methodReference.FullName == compareString)
                {
                    if (usingDebug)
                        DebugFile.WriteLine($"this is getter Validator<T>", consoleSpacing);
                    return true;
                }
                else
                {
                    if (usingDebug)
                        DebugFile.WriteLine($"compare string \"{compareString}\" - NO EQUAL", consoleSpacing);
                }
            }

            return false;
        }

        public static bool IsMethodGetterValidatorGroup(this MethodDefinition methodDefinition, bool usingDebug = true, int consoleSpacing = 0)
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
                {
                    DebugFile.WriteLine($"check is method <{methodDefinition.Name}> getter ValidatorGroup => {methodReference.FullName}", consoleSpacing);
                    consoleSpacing++;
                }

                var returnTypeName = methodDefinition.ReturnType.FullName;
                if (usingDebug)
                    DebugFile.WriteLine($"return type: {returnTypeName}", consoleSpacing);

                if (methodReference.FullName == ModuleWeaver.METHOD_BUILD_GROUP_NAME)
                {
                    if (usingDebug)
                        DebugFile.WriteLine($"this is getter ValidatorGroup", consoleSpacing);
                    return true;
                }
                else
                {
                    if (usingDebug)
                        DebugFile.WriteLine($"compare string \"{ModuleWeaver.METHOD_BUILD_GROUP_NAME}\" - NO EQUAL", consoleSpacing);
                }
            }

            return false;
        }
    }
}
