using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestGroups
    {
        public Validator<string> Test1 => Validator<string>.Build()
            .UsingRule(x => x.Length >= 2, "Error");

        public Validator<string> Test2 => Validator<string>.Build()
            .UsingRule(x => x.Length >= 5, "Error");

        public ValidatorGroup TestGroup => ValidatorGroup.Build()
            .Include(Test1)
            .Include(Test2);

        public ValidatorGroup EmptyGroup => ValidatorGroup.Build();

        // тест работу IsValid
        [TestMethod]
        public void Test_IsValid()
        {
            Test1.Value = "AB";
            Test2.Value = "ABCDE";
            Assert.AreEqual(true, TestGroup.IsValid);
            Assert.AreEqual(false, TestGroup.HasErrors);
        }

        // тест работу HasErrors
        [TestMethod]
        public void Test_HasErrors()
        {
            Test1.Value = "AB";
            Test2.Value = "AB";
            Assert.AreEqual(false, TestGroup.IsValid);
            Assert.AreEqual(true, TestGroup.HasErrors);
        }

        // пустая группа должна быть невалидной и несодержать ошибок
        [TestMethod]
        public void Test_DefaultInvalid()
        {
            Assert.AreEqual(false, EmptyGroup.IsValid);
            Assert.AreEqual(false, EmptyGroup.HasErrors);
        }
    }
}
