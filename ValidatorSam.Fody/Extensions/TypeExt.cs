using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ValidatorSam.Fody.Postprocessing;
using ValidatorSam.Fody.Utils;

namespace ValidatorSam.Fody.Extensions
{
    public static class TypeExt
    {
        public static bool IsUsingValidatorSam(this TypeDefinition type)
        {
            bool isAny = false;
            foreach (var item in type.Properties)
            {
                if (item.SetMethod != null)
                    continue;

                if (item.GetMethod.IsMethodAutoGen())
                    #if DEBUG
                    isAny = true;
                    #else
                    return true;
                    #endif
            } 

            return isAny;
        }

        public static void InjectFixInClass(this TypeDefinition classType, BaseModuleWeaver weaver)
        {
            var rand = new Random();
            var getters = classType.Properties.Where(x => x.IsAutoValidator()).ToArray();

            foreach (var getterProp in getters)
            {
                int id = rand.Next(0,1000000);
                var originGetterMethod = getterProp.GetMethod;
                var methodReturnType = originGetterMethod.ReturnType;

                // Copy and clear
                var instructions = new List<Instruction>();
                foreach (var item in originGetterMethod.Body.Instructions)
                    instructions.Add(item);
                originGetterMethod.Body.Instructions.Clear();

                // todo Нужно ли копировать инструкции?
                var vars = new List<VariableDefinition>();
                foreach (var item in originGetterMethod.Body.Variables)
                    vars.Add(item);
                originGetterMethod.Body.Variables.Clear();

                // field
                var field = new FieldDefinition($"{originGetterMethod.Name}_FODY_{id}_field", FieldAttributes.Private, methodReturnType);
                classType.Fields.Add(field);

                // method
                var methodName = $"{originGetterMethod.Name}_FODY_{id}";
                var flags = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
                var newGetterMethod = new MethodDefinition(methodName, flags, methodReturnType);

                foreach (var item in instructions)
                    newGetterMethod.Body.Instructions.Add(item);

                foreach (var item in vars)
                    newGetterMethod.Body.Variables.Add(item);

                originGetterMethod.GenerateBody(newGetterMethod, field);
                classType.Methods.Add(newGetterMethod);
            }
        }

        public static TypeReference MakeGeneric(this TypeReference self, params TypeReference[] arguments)
        {
            if (self.GenericParameters.Count != arguments.Length)
                throw new ArgumentException();

            var instance = new GenericInstanceType(self);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);

            return instance;
        }
    }
}
