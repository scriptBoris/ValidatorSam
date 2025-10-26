using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTestNuget
{
    public class MockController
    {
        private readonly IValidator _validator;

        public MockController(IValidator validator)
        {
            _validator = validator;
        }

        public void SimulateInput(char v)
        {
            _validator.RawValue += v;

            Console.WriteLine($"Current RawValue: {_validator.RawValue}");
        }
    }
}
