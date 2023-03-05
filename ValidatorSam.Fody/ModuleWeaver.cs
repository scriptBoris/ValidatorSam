using Fody;
using System;
using System.Collections.Generic;
using ValidatorSam.Fody.Postprocessing;
using ValidatorSam.Fody.Utils;
using ValidatorSam.Fody.Extensions;
using Mono.Cecil;
using System.Reflection;

namespace ValidatorSam.Fody
{
    public class ModuleWeaver : BaseModuleWeaver
    {
        public static ModuleWeaver Instance { get; private set; }
        public TypeReference ValidatorT_TypeRefrence { get; private set; }

        public override void Execute()
        {
            Instance = this;
            string finish = null;
            Debug("Start: postprocessing...");

            try
            {
                ValidatorT_TypeRefrence = GetTypeRefValidatorT();
                if (ValidatorT_TypeRefrence == null)
                    throw new Exception("Not found Validator<T> class");
                else
                    Debug($"find type reference Validator<T>");

                var finder = new Finder(this);
                var types = finder.Find();
                foreach (var type in types)
                    Debug($"find class: {type.Name}");

                if (types.Length == 0)
                    finish = "No one match validators";

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

        public TypeReference GetTypeRefValidatorT()
        {
            AssemblyNameReference validatorDll = null;
            foreach (var item in ModuleDefinition.AssemblyReferences)
            {
                Debug($"find Validator.dll assembly::{item.Name}");
                if (item.Name == "ValidatorSam")
                    validatorDll = item;
            }

            var asm = AssemblyResolver.Resolve(validatorDll);
            foreach (var item in asm.MainModule.Types)
            {
                Debug($"find validator<T> on {item.Name}");
                if (item.Name == @"Validator`1")
                {
                    return item;
                }
            }

            return null;
        }
    }
}
