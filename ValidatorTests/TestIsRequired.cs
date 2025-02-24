using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestIsRequired
    {
        // default error
        public Validator<string> Test1 => Validator<string>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckDefaultError()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Test1.CheckValid();
            Assert.AreEqual("Required", Test1.TextError);
        }

        // static error
        public Validator<string> UserName => Validator<string>.Build()
            .UsingRequired("static required error");
        [TestMethod]
        public void CheckIsRequiredStaticText()
        {
            UserName.CheckValid();
            Assert.AreEqual("static required error", UserName.TextError);
        }

        // dynamic error
        public Validator<int?> UserAge => Validator<int?>.Build()
            .UsingRequired(() => "dynamic required error");
        [TestMethod]
        public void CheckIsRequiredText()
        {
            UserAge.CheckValid();
            Assert.AreEqual("dynamic required error", UserAge.TextError);
        }
    }
}
