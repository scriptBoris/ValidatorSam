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
        public static void GenerateBody(this MethodDefinition method, MethodDefinition originalGETmethod, FieldDefinition field)
        {
            // variable object
            //var boolType = originalGETmethod.Module.ImportReference(typeof(bool));
            //var boolVariable = new VariableDefinition(boolType);
            //method.Body.Variables.Add(boolVariable);

            //var testVariable = new VariableDefinition(field.FieldType);
            //method.Body.Variables.Add(testVariable);
            //method.Body.InitLocals = true;
            //method.Body.MaxStackSize = 26;

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
            il.Emit(OpCodes.Call, originalGETmethod);
            il.Emit(OpCodes.Stfld, field);

            // ENDIF
            il.Append(skip);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);
        }

        //public static void GenerateBody(this MethodDefinition method, MethodDefinition originalGETmethod, FieldDefinition testField)
        //{
        //    // variable object
        //    var testVariable = new VariableDefinition(testField.FieldType);
        //    method.Body.Variables.Add(testVariable);

        //    var il = method.Body.GetILProcessor();
        //    method.Body.InitLocals = true;
        //    method.Body.MaxStackSize = 26;

        //    // Создаем метки возврата и конца блока
        //    Instruction returnLabel = il.Create(OpCodes.Nop);
        //    Instruction endOfMethod = il.Create(OpCodes.Nop);

        //    // if (test == null)
        //    Instruction ifInstruction = il.Create(OpCodes.Nop);
        //    il.Emit(OpCodes.Ldarg_0);
        //    il.Emit(OpCodes.Ldfld, testField);
        //    il.Emit(OpCodes.Brtrue_S, ifInstruction);
        //    il.Emit(OpCodes.Call, originalGETmethod);
        //    il.Emit(OpCodes.Stloc_0);

        //    // return test;
        //    il.Emit(OpCodes.Ldloc_0);
        //    il.Emit(OpCodes.Br_S, returnLabel);

        //    // Метка if
        //    il.Append(ifInstruction);
        //    il.Emit(OpCodes.Ldarg_0);
        //    il.Emit(OpCodes.Ldfld, testField);

        //    // Создаем метку возврата
        //    il.Append(returnLabel);
        //    il.Emit(OpCodes.Ret);
        //}

        //public static void GenerateBody(MethodDefinition method, MethodDefinition originalGETmethod, FieldDefinition testField)
        //{
        //    if (method.Body == null)
        //        method.Body = new MethodBody(method);

        //    // variable object
        //    var testVariable = new VariableDefinition(testField.FieldType);
        //    method.Body.Variables.Add(testVariable);

        //    var il = method.Body.GetILProcessor();

        //    il.Emit(OpCodes.Ldarg_0); // загружаем this на стек
        //    il.Emit(OpCodes.Ldfld, testField); // загружаем поле test на стек
        //    var ifNull = il.Create(OpCodes.Nop);
        //    il.Emit(OpCodes.Brtrue_S, ifNull); // если test не null, переходим на ifNull

        //    // если test == null
        //    il.Emit(OpCodes.Ldarg_0); // загружаем this на стек
        //    il.Emit(OpCodes.Call, originalGETmethod); // вызываем TestGET
        //    il.Emit(OpCodes.Stfld, testField); // сохраняем результат в поле test

        //    il.Append(ifNull);
        //    il.Emit(OpCodes.Ldarg_0); // загружаем this на стек
        //    il.Emit(OpCodes.Ldfld, testField); // загружаем поле test на стек
        //    il.Emit(OpCodes.Ret); // возвращаем значение на стек
        //}
    }
}
