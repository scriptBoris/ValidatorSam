using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestWeaving
    {
        // weaving name
        public Validator<string> Name => Validator<string>.Build();

        [TestMethod]
        public void TestName()
        {
            Assert.AreEqual(Name.Name, nameof(Name));
        }

        // weaving singleton
        public Validator<string> Singleton => Validator<string>.Build();

        [TestMethod]
        public void TestSingletion()
        {
            Singleton.Value = "Hello";
            Assert.AreEqual(Singleton.Value, "Hello");
        }

        // тест на singleton для ValidatorGroup
        public Validator<string> Test3 => Validator<string>.Build()
            .UsingRule(x => x.Value.Length >= 5, "Error");

        public Validator<string> Test4 => Validator<string>.Build()
            .UsingRule(x => x.Value.Length >= 10, "Error");

        public ValidatorGroup TestGroup => ValidatorGroup.Build()
            .Include(Test3)
            .Include(Test4);

        [TestMethod]
        public void TestValidatorGroup_Singleton()
        {
            var group1 = TestGroup;
            var group2 = TestGroup;
            Test3.Value = "12345";
            Test4.Value = "1234567890";

            bool check1 = group1.IsValid;
            bool check2 = group2.IsValid;

            Assert.AreEqual(true, check1 == true && check2 == true);
        }
    }
}
