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

        // Тест на проверку получения статического текста ошибки
        public Validator<string> EmailStatic => Validator<string>.Build()
            .UsingRule(x => x == "test@email.com", "static text error");
        [TestMethod]
        public void TestStaticTextError()
        {
            EmailStatic.Value = "bad@email.com";
            Assert.AreEqual("static text error", EmailStatic.TextError);
        }

        // Тест на проверку получения динамического текста ошибки
        public Validator<string> EmailDynamic => Validator<string>.Build()
            .UsingRule(x => x == "test@email.com", () => "dynamic error");
        [TestMethod]
        public void TestDynamicTextError()
        {
            EmailDynamic.Value = "bad@email.com";
            Assert.AreEqual("dynamic error", EmailDynamic.TextError);
        }

        // Тест на проверку получения динамического текста ошибки у SafeRule
        public Validator<string?> EmailDynamicSafe => Validator<string?>.Build()
            .UsingSafeRule(x => x == "test@email.com", () => "dynamic error");
        [TestMethod]
        public void TestSafeDynamicTextError()
        {
            EmailDynamicSafe.Value = "bad@email.com";
            Assert.AreEqual("dynamic error", EmailDynamicSafe.TextError);
        }
    }
}
