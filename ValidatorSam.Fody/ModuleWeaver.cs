using Fody;
using System;
using System.Collections.Generic;
using ValidatorSam.Fody.Postprocessing;
using ValidatorSam.Fody.Utils;
using ValidatorSam.Fody.Extensions;
using Mono.Cecil;
using System.Reflection;
using System.Linq;

namespace ValidatorSam.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        public const string METHOD_BUILD_NAME = "ValidatorSam.ValidatorBuilder`1<T> ValidatorSam.Validator`1::Build()";
        public const string TYPE_VALIDATORTSAM = @"Validator`1";

        public static ModuleWeaver Instance { get; private set; }

        public AssemblyNameReference ValidatorSam_Dll { get; private set; }   
        public TypeReference ValidatorT_TypeRefrence { get; private set; }
        public MethodReference MethodBuild { get; private set; }

        public override void Execute()
        {
            Instance = this;
            string finish = null;
            Debug("Start: postprocessing...");

            try
            {
                // find dll
                ValidatorSam_Dll = GetValidatorSam(ModuleDefinition);
                if (ValidatorSam_Dll == null)
                    throw new Exception("Not found ValidatorSam.dll");

                // find validatorT type
                ValidatorT_TypeRefrence = GetTypeRefValidatorT(ValidatorSam_Dll);
                if (ValidatorT_TypeRefrence == null)
                    throw new Exception("Not found Validator<T> class");

                MethodBuild = GetMethodBuild(ValidatorT_TypeRefrence);
                if (MethodBuild == null)
                    throw new Exception("Not found Validator<T>.Build() method");

                // find classes
                var finder = new Finder(this);
                var types = finder.Find();
                foreach (var type in types)
                    Debug($"find class: {type.Name}");

                if (types.Length == 0)
                    finish = "No one match validators";

                // inject
                foreach (var item in types)
                    item.InjectFixInClass(this);
            }
            catch (Exception ex)
            {
                string exMSG = "";
                var exx = ex;
                while(exx != null)
                {
                    exMSG += exx.Message;
                    exx = exx.InnerException;
                }

                finish = "FAIL ";
                finish += $" [{exMSG}]\n";
                finish += ex.StackTrace;
                WriteError(finish);
            }
            finally
            {
                Debug($"Finish: {finish ?? "SUCCESS"}");
            }
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return new string[] 
            {
                "ValidatorSam.dll"
            };
        }

        public void Debug(string message)
        {
            DebugFile.WriteLine(message);
        }


        public AssemblyNameReference GetValidatorSam(ModuleDefinition module)
        {
            foreach (var item in module.AssemblyReferences)
            {
                Debug($"find Validator.dll assembly::{item.Name}");
                if (item.Name == "ValidatorSam")
                {
                    return item;
                }
            }

            return null;
        }

        public TypeReference GetTypeRefValidatorT(AssemblyNameReference dll)
        {
            var asm = AssemblyResolver.Resolve(dll);
            foreach (var item in asm.MainModule.Types)
            {
                Debug($"find validator<T> on {item.Name}");
                if (item.Name == TYPE_VALIDATORTSAM)
                {
                    return item;
                }
            }

            return null;
        }

        public MethodReference GetMethodBuild(TypeReference type)
        {
            foreach(var item in type.Resolve().Methods)
            {
                Debug($"DEBUG find method on {item.FullName}");
                if (item.FullName == METHOD_BUILD_NAME)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
