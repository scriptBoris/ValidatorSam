using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;
using ValidatorTests.Enums;

namespace ValidatorTests
{
    [TestClass]
    public class TestInitValues
    {
        private bool _valueChanged;
        private bool _valueChangedEmum;

        public TestInitValues()
        {
        }

        // Инициализированное Value не должно инвокать UsingValueChangeListener
        public Validator<string?> UserEmail => Validator<string?>.Build()
            .UsingValue("boris@gmail.com")
            .UsingValueChangeListener(x =>
            {
                _valueChanged = true;
            })
            .UsingRequired();

        [TestMethod]
        public void CheckNoInvokeValueChangeListener()
        {
            Assert.AreEqual(false, _valueChanged);
        }

        // Инициализированное Value не должно инвокать UsingValueChangeListener
        // (но уже для типа Enum)
        public Validator<UserRoles> UserRole => Validator<UserRoles>.Build()
            .UsingValue(UserRoles.Admin)
            .UsingValueChangeListener(x =>
            {
                _valueChangedEmum = true;
            })
            .UsingRequired();

        [TestMethod]
        public void CheckNoInvokeValueChangeListenerEnum()
        {
            Assert.AreEqual(false, _valueChangedEmum);
        }

        // проверка на изначально "плохие" данные.
        // суть в том, валидатор должен отображать данные указанные через UsingValue(...)
        // с высоким приоритетом, даже несмотря на ограничения 18-100
        public Validator<int> UserAge => Validator<int>.Build()
            .UsingValue(-20)
            .UsingLimitations(18, 100)
            .UsingRequired();
        [TestMethod]
        public void CheckBadInitData()
        {
            Assert.AreEqual(UserAge.Value, -20);
            Assert.AreEqual(false, UserAge.IsValid);
        }


        // По умолчанию IsValid должен быть false!
        // Даже если инициализированное значение имеет валидные данные
        public Validator<string> DefaultName => Validator<string>.Build()
            .UsingValue("valid")
            .UsingRule(x => x == "valid", "ERROR");
        [TestMethod]
        public void CheckDefaultIsValid()
        {
            string value = DefaultName.Value;
            Assert.AreEqual(false, DefaultName.IsValid);
        }

        // Проверка на IsVisualValid
        // По умолчанию IsVisualValid должен быть true, даже если инициализированное значение
        // имеет невалидные данные
        public Validator<string> DefaultIsVisualValid => Validator<string>.Build()
            .UsingValue("invalid")
            .UsingRule(x => x == "valid", "ERROR");
        [TestMethod]
        public void CheckDefaultIsVisualValid()
        {
            string value = DefaultName.Value;
            Assert.AreEqual(true, DefaultName.IsVisualValid);
        }
    }
}