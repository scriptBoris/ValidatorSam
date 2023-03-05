using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ValidatorSam.Fody.Utils;

namespace ValidatorSam.Fody.Postprocessing
{
    internal static class SingletonGenerator
    {
        /// <summary>
        /// Генерирует следующий код: <br/>
        /// if (field == null) <br/>
        ///     field = SetMethod(); <br/>
        /// return field;
        /// </summary>
        public static void GenerateBody(this MethodDefinition method, MethodDefinition setMethod, FieldDefinition field)
        {
            GenerateBody_ClassType(method, setMethod, field);
            return;
            DebugFile.WriteLine($"generate body for {method.FullName}");
            var fieldTypeT = field.FieldType;

            TypeReference genericType;
            if (fieldTypeT is GenericInstanceType gt)
            {
                genericType = gt.GenericArguments.First();
            }
            else
            {
                throw new ArgumentException($"method {method.FullName} fail get generic type");
            }

            if (genericType.IsValueType)
            {
                DebugFile.WriteLine($"   generic type is VALUE_TYPE [{genericType.FullName}]");
                GenerateBody_ValueType(method, setMethod, field);
            }
            else
            {
                DebugFile.WriteLine($"   generic type is HANDLE [{genericType.FullName}]");
                GenerateBody_ClassType(method, setMethod, field);
            }
        }

        /// <summary>
        /// Генерирует следующий код: <br/>
        /// if (field == null) <br/>
        ///     field = SetMethod(); <br/>
        /// return field;
        /// </summary>
        private static void GenerateBody_ClassType(MethodDefinition method, MethodDefinition setMethod, FieldDefinition field)
        {
            var il = method.Body.GetILProcessor();
            var skip = Instruction.Create(OpCodes.Nop);

            // IF
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Brtrue_S, skip);

            // SET
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, setMethod);
            il.Emit(OpCodes.Stfld, field);

            // ENDIF
            il.Append(skip);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);
        }

        private static void GenerateBody_ValueType(MethodDefinition method, MethodDefinition setMethod, FieldDefinition field)
        {
            var il = method.Body.GetILProcessor();
            method.Body.Variables.Add(new VariableDefinition(method.Module.ImportReference(typeof(bool))));
            method.Body.Variables.Add(new VariableDefinition(field.FieldType));

            var skip001a = Instruction.Create(OpCodes.Nop);
            var skip0023 = Instruction.Create(OpCodes.Nop);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Stloc_0);

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Brfalse_S, skip001a);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, setMethod);
            il.Emit(OpCodes.Stfld, field);

            il.Append(skip001a);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Br_S, skip0023);

            il.Append(skip0023);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
        }
    }
}
