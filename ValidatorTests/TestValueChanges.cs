using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;
using ValidatorTests.Enums;
using ValidatorTests.Supports;

namespace ValidatorTests
{
    [TestClass]
    public class TestValueChanges
    {
        private MockObject _initObject = new();
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

        // проверка работы метода CheckChanges()
        public Validator<bool> BoolProperty => Validator<bool>.Build()
            .UsingValue(false);

        public Validator<int> IntProperty => Validator<int>.Build()
            .UsingValue(123);

        public Validator<MockObject> MockObjectProperty => Validator<MockObject>.Build()
            .UsingValue(_initObject);

        public Validator<MockObject> MockObjectProperty2 => Validator<MockObject>.Build()
            .UsingValue(_initObject);

        public Validator<string> StringProperty => Validator<string>.Build()
            .UsingValue("init value");

        public Validator<string> StringProperty2 => Validator<string>.Build()
            .UsingValue("init value");

        public Validator<string> StringProperty3 => Validator<string>.Build()
            .UsingValue("init value");

        [TestMethod]
        public void CheckManualChanges()
        {
            // проверка enum
            UserRole.Value = UserRoles.None;
            bool isUserRoleChanged = UserRole.CheckChanges();
            Assert.AreEqual(true, isUserRoleChanged);

            // проверка bool
            BoolProperty.Value = true;
            bool isBoolChanged = BoolProperty.CheckChanges();
            Assert.AreEqual(true, isBoolChanged);

            // проверка int
            IntProperty.Value = 1000;
            bool isIntChanged = IntProperty.CheckChanges();
            Assert.AreEqual(true, isIntChanged);

            // проверка экземпляра класса
            MockObjectProperty.Value = new MockObject();
            bool isMockObjectChanged = MockObjectProperty.CheckChanges();
            Assert.AreEqual(true, isMockObjectChanged);

            // контрольная проверка экземпляра класса
            bool isMockObjectChanged2 = MockObjectProperty2.CheckChanges();
            Assert.AreEqual(false, isMockObjectChanged2);

            // проверка string
            StringProperty.Value = "new value";
            bool isStringChanged = StringProperty.CheckChanges();
            Assert.AreEqual(true, isStringChanged);

            // контрольная проверка string
            bool isStringChanged2 = StringProperty2.CheckChanges();
            Assert.AreEqual(false, isStringChanged2);

            // контрольная проверка string (указываем те же данные что и в init value)
            StringProperty3.Value = "init value";
            bool isStringChanged3 = StringProperty3.CheckChanges();
            Assert.AreEqual(false, isStringChanged3);
        }
    }
}
