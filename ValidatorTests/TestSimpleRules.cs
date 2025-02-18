using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestSimpleRules
    {
        public Validator<int> Property1 => Validator<int>.Build()
            .UsingRule(x => x > 10, "valus is less than 10");

        [TestMethod]
        public void TestRule()
        {
            Property1.Value = 20;
            Assert.AreEqual(true, Property1.IsValid);
            Assert.AreEqual(null, Property1.TextError);
        }

        [TestMethod]
        public void TestRule2()
        {
            Property1.Value = 5;
            Assert.AreEqual(false, Property1.IsValid);
            Assert.AreEqual("valus is less than 10", Property1.TextError);
        }

        [TestMethod]
        public void TestRule3()
        {
            Property1.Value = 5;
            Assert.AreEqual(true, !Property1.IsValid);
            Assert.AreEqual("valus is less than 10", Property1.TextError);
        }
    }
}
