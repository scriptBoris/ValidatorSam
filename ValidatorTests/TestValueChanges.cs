﻿using System;
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
        private bool _valueChanged_2;
        private bool _valueChanged_3;

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

        // проверка что данные НЕ были изменены, т.к. указывается те же данные,
        // что уже используются в самом валидаторе
        // (Вариант с событием)
        [TestMethod]
        public void CheckWillNotChanged_Event()
        {
            UserRole.ValueChanged += (o, e) =>
            {
                _valueChanged_2 = true;
            };
            UserRole.Value = UserRoles.Admin;
            Assert.AreEqual(false, _valueChanged_2);
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

        public Validator<MockObject> MockObjectProperty3 => Validator<MockObject>.Build()
            .UsingRequired();

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

        [TestMethod]
        public void CheckCaseSilentValueChanged()
        {
            // контрольная проверка экземпляра класса
            // (мой кейс, когда изначально "пустое" проперти сетапило значение из загруженных
            // данных из сервера, но при сетапе событие не инвокалось)
            MockObjectProperty3.ValueChanged += MockObjectProperty3_ValueChanged;
            MockObjectProperty3.Value = new MockObject();
            bool isMockObjectChanged3 = MockObjectProperty3.CheckChanges();
            Assert.AreEqual(true, isMockObjectChanged3);
            Assert.AreEqual(true, _valueChanged_3);
        }

        private void MockObjectProperty3_ValueChanged(Validator invoker, ValidatorValueChangedArgs args)
        {
            _valueChanged_3 = true;
        }
    }
}
