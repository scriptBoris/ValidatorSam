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
        public static bool IsMethodAutoGen(this MethodDefinition methodDefinition)
        {
            if (methodDefinition == null)
                return false;

            if (!methodDefinition.HasBody)
                return false;

            var il = methodDefinition.Body.Instructions.First();
            if (il.OpCode == OpCodes.Call &&
                il.Operand is MethodReference methodReference)
            {
                DebugFile.WriteLine($"{methodDefinition.Name} => {methodReference.FullName}");

                if (methodReference.FullName == "ValidatorSam.ValidatorBuilder`1<!0> ValidatorSam.Validator`1<System.String>::Build()")
                    return true;
            }


            return false;
        }

        public static MethodReference MakeGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType)
            {
                DeclaringType = self.DeclaringType.MakeGeneric(arguments),
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention,
            };

            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var generic_parameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            return reference;
        }
    }
}
