using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTestNuget
{
    public class MockClass
    {
        public Validator<string> StringValidator => Validator<string>.Build()
            .UsingRule(x => x == "success", "error!")
            .UsingRequired();

        public bool CheckValid()
        {
            StringValidator.Value = "success";
            return StringValidator.IsValid;
        }
    }
}
