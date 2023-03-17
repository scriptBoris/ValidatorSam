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

            var root = weaver.ModuleDefinition.Types;
            //var root = weaver.ModuleDefinition.Types.Where(x => x.IsUsingValidatorSam());

            foreach (var item in root)
            {
                var nesteds = FindInner(item);
                list.AddRange(nesteds);
            }

            return list.ToArray();
        }

        private TypeDefinition[] FindInner(TypeDefinition typeDefinition)
        {
            var list = new List<TypeDefinition>();

            if (typeDefinition.IsUsingValidatorSam())
                list.Add(typeDefinition);

            var nestedTypes = typeDefinition.NestedTypes;

            foreach (var type in nestedTypes)
            {
                var nesteds = FindInner(type);
                list.AddRange(nesteds);
            }

            return list.ToArray();
        }
    }
}
