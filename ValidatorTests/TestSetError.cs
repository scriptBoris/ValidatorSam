using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestSetError
    {
        public Validator<string> Test => Validator<string>.Build();
           
        // Тест на сценарий:
        // - Пользователь никак не взаимодействовал с полем ввода,
        // - Нажал на кнопку далее
        // - Сработал скрипт проверки и установил валидаторам ошибку через SetError(...)
        [TestMethod]
        public void CheckSetError()
        {
            Test.SetError("External test error");
            Assert.AreEqual(false, Test.IsValid);
            Assert.AreEqual(false, Test.IsVisualValid);
            Assert.AreEqual("External test error", Test.TextError);
        }

        // Тест на сценарий:
        // - Пользователь взаимодействовал с полем ввода,
        // - Нажал на кнопку далее
        // - Сработал скрипт проверки и установил валидаторам ошибку через SetError(...)
        [TestMethod]
        public void CheckSetErrorWithInputRawValue()
        {
            Test.RawValue = "simple data";
            Test.SetError("External test error");
            Assert.AreEqual(false, Test.IsValid);
            Assert.AreEqual(false, Test.IsVisualValid);
            Assert.AreEqual("External test error", Test.TextError);
        }
    }
}