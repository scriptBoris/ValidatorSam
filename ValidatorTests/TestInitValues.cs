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

        public Validator<string?> UserEmail => Validator<string?>.Build()
            .UsingValue("boris@gmail.com")
            .UsingValueChangeListener(x =>
            {
                _valueChanged = true;
            })
            .UsingRequired();


        public Validator<UserRoles> UserRole => Validator<UserRoles>.Build()
            .UsingValue(UserRoles.Admin)
            .UsingValueChangeListener(x =>
            {
                _valueChanged = true;
            })
            .UsingRequired();

        [TestMethod]
        public void CheckNoInvokeValueChangeListener()
        {
            var value = UserEmail.Value;
            Assert.AreEqual(false, _valueChanged, $"GGG");
        }

        [TestMethod]
        public void CheckNoInvokeValueChangeListenerEnum()
        {
            var value = UserRole.Value;
            Assert.AreEqual(false, _valueChangedEmum, $"GGG");
        }
    }
}