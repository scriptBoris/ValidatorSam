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
        public const string METHOD_SETNAME_NAME = "System.Void ValidatorSam.Validator::SetNameByFodyPostprocessing(System.String)";
        public const string TYPE_VALIDATORTSAM_T = @"Validator`1";
        public const string TYPE_VALIDATORTSAM = @"Validator";

        public static ModuleWeaver Instance { get; private set; }

        public AssemblyNameReference ValidatorSam_Dll { get; private set; }   
        public TypeReference ValidatorT_TypeRefrence { get; private set; }
        public TypeReference Validator_TypeRefrence { get; private set; }
        public MethodReference MethodBuild { get; private set; }
        public MethodReference MethodSetName { get; private set; }

        public override void Execute()
        {
            Instance = this;
            string finish = null;
            DebugFile.ClearFile();
            Debug("Start: postprocessing...");

            try
            {
                // find dll
                ValidatorSam_Dll = FindValidatorSamDll(ModuleDefinition);
                if (ValidatorSam_Dll == null)
                    throw new Exception("Not found ValidatorSam.dll");
                Debug("OK: ValidatorSam.dll is matched!");

                // find validatorT type
                ValidatorT_TypeRefrence = FindTypeRef(ValidatorSam_Dll, TYPE_VALIDATORTSAM_T);
                if (ValidatorT_TypeRefrence == null)
                    throw new Exception("Not found Validator<T> class");
                Debug("OK: Validator<T> class is matched!");

                // find validator type
                Validator_TypeRefrence = FindTypeRef(ValidatorSam_Dll, TYPE_VALIDATORTSAM);
                if (Validator_TypeRefrence == null)
                    throw new Exception("Not found Validator class");
                Debug("OK: Validator class is matched!");

                // find build method
                MethodBuild = FindMethod(ValidatorT_TypeRefrence, METHOD_BUILD_NAME);
                if (MethodBuild == null)
                    throw new Exception("Not found Validator<T>.Build() method");
                Debug($"OK: method <Build> is matched!");

                // find set name method
                MethodSetName = FindMethod(Validator_TypeRefrence, METHOD_SETNAME_NAME);
                if (MethodSetName == null)
                    throw new Exception($"Not found {METHOD_SETNAME_NAME} method");
                Debug($"OK: method <SetName> is matched!");

                MethodSetName = this.ModuleDefinition.ImportReference(MethodSetName);

                // find classes
                var finder = new Finder(this);
                var types = finder.Find();

                Debug($"SEARCH RESULT:");
                foreach (var type in types)
                    Debug($"- find class for injection: {type.FullName}");

                if (types.Length == 0)
                    finish = "- No one match validators";

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


        public AssemblyNameReference FindValidatorSamDll(ModuleDefinition module)
        {
            foreach (var item in module.AssemblyReferences)
            {
                Debug($"try find Validator.dll in assembly: <{item.Name}>...");
                if (item.Name == "ValidatorSam")
                {
                    return item;
                }
            }

            return null;
        }

        public TypeReference FindTypeRef(AssemblyNameReference dll, string findTypeFullName)
        {
            Debug($"try find class <{findTypeFullName}> in dll: <{dll.Name}>...");
            var asm = AssemblyResolver.Resolve(dll);
            foreach (var item in asm.MainModule.Types)
            {
                Debug($"compare {findTypeFullName} with {item.Name}");
                if (item.Name == findTypeFullName)
                {
                    return item;
                }
            }

            return null;
        }

        public MethodReference FindMethod(TypeReference type, string findMethodFullName)
        {
            Debug($"try find method <{findMethodFullName}> in class: <{type.FullName}>...");
            foreach (var item in type.Resolve().Methods)
            {
                Debug($"compare <{findMethodFullName}> with <{item.FullName}>");
                if (item.FullName == findMethodFullName)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
