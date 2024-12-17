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
    public class TestValueChanges
    {
        private bool _valueChanged;

        public Validator<UserRoles> UserRole => Validator<UserRoles>.Build()
            .UsingValue(UserRoles.Admin)
            .UsingValueChangeListener(x =>
            {
                _valueChanged = true;
            })
            .UsingRequired();

        [TestMethod]
        public void Check()
        {
            var value = UserRole.Value = UserRoles.Admin;
            Assert.AreEqual(false, _valueChanged, $"GGG");
        }
    }
}
