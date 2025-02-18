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

        // проверка что данные НЕ были изменены, т.к. указывается те же данные,
        // что уже используются в самом валидаторе
        [TestMethod]
        public void CheckWillNotChanged()
        {
            UserRole.Value = UserRoles.Admin;
            Assert.AreEqual(false, _valueChanged);
        }

        // проверка что данные были изменены, т.к. указывается другие данные,
        // чем те что уже используются в самом валидаторе
        [TestMethod]
        public void CheckWillChanged()
        {
            UserRole.Value = UserRoles.None;
            Assert.AreEqual(true, _valueChanged);
        }
    }
}
