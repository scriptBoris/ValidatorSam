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
    }
}
