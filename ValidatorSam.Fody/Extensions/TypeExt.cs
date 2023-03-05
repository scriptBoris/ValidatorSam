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
                var originGetterMethod = getterProp.GetMethod;
                int id = rand.Next(0,1000000);
                var methodReturnType = originGetterMethod.ReturnType;

                // field
                var field = new FieldDefinition($"{originGetterMethod.Name}_FODY_{id}_field", FieldAttributes.Private, methodReturnType);
                classType.Fields.Add(field);

                // method
                var methodName = $"{originGetterMethod.Name}_FODY_{id}";
                //var flags = MethodAttributes.Public |
                //    MethodAttributes.HideBySig | 
                //    MethodAttributes.SpecialName |
                //    MethodAttributes.Virtual | 
                //    MethodAttributes.Final |
                //    MethodAttributes.NewSlot;
                var flags = MethodAttributes.Public |
                    MethodAttributes.HideBySig |
                    MethodAttributes.SpecialName;
                var newGetterMethod = new MethodDefinition(methodName, flags, methodReturnType);
                newGetterMethod.GenerateBody(originGetterMethod, field);

                classType.Methods.Add(newGetterMethod);

                // replace
                getterProp.GetMethod = newGetterMethod;
                originGetterMethod.Attributes = MethodAttributes.Public;

                // rename
                string ogname = originGetterMethod.Name;
                originGetterMethod.Name = newGetterMethod.Name;
                newGetterMethod.Name = ogname;
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
