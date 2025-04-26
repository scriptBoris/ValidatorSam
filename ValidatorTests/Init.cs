using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam.Core;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
namespace ValidatorTests
{
    [TestClass]
    public static class AssemblyInit
    {
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            ValidatorLocalization.CultureInfo = new System.Globalization.CultureInfo("en-US");
        }
    }
}