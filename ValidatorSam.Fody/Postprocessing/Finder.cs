using Fody;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValidatorSam.Fody.Extensions;

namespace ValidatorSam.Fody.Postprocessing
{
    public class Finder
    {
        private readonly BaseModuleWeaver weaver;
        public Finder(BaseModuleWeaver weaver)
        {
            this.weaver = weaver;
        }

        public TypeDefinition[] Find()
        {
            var list = new List<TypeDefinition>();
            
            foreach (var type in weaver.ModuleDefinition.Types.Where(x => x.IsUsingValidatorSam()))
            {
                list.Add(type);
            }

            return list.ToArray();
        }

    }
}
