using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;
using ValidatorSam.Core;

namespace ValidatorTests
{
    [TestClass]
    [DoNotParallelize]
    public class TestIsRequired
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ValidatorLocalization.CultureInfo = new System.Globalization.CultureInfo("en-US");
        }

        // default error
        public Validator<string> Test1 => Validator<string>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckDefaultError()
        {
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

        // bool is required
        public Validator<bool> AgreementTerms => Validator<bool>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckAgreementTerms()
        {
            AgreementTerms.CheckValid();
            Assert.AreEqual(false, AgreementTerms.IsValid);
        }

        // bool is required2
        public Validator<bool> AgreementTerms2 => Validator<bool>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckAgreementTerms2()
        {
            AgreementTerms2.Value = true;
            AgreementTerms2.CheckValid();
            Assert.AreEqual(true, AgreementTerms2.IsValid);
        }

        // Тест когда требуется IsRequired, но с "сервера приходит пустые данные"
        public Validator<string> OrderCode => Validator<string>.Build()
            .UsingRequired();
        [TestMethod]
        public void InitValueWithIsRequired()
        {
            bool error = false;
            OrderCode.ErrorChanged += (o, e) =>
            {
                error = e.IsShow;
            };

            OrderCode.SetValueAsRat(null, RatModes.InitValue | RatModes.SkipPreprocessors | RatModes.SkipValidation);
            Assert.AreEqual(false, error);
        }
    }
}
