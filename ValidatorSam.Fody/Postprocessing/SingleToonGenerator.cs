using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var il = method.Body.GetILProcessor();
            var skip = Instruction.Create(OpCodes.Nop);

            // IF
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            //il.Emit(OpCodes.Brfalse_S, skip);
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
    }
}
