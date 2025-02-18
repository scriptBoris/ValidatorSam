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
    }
}