using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestVisualValid
    {
        public Validator<int> TestValidator1 => Validator<int>.Build()
            .UsingRule(x => x.Value > 0, "ERROR");

        // Изначально IsVisualValid должен быть true
        [TestMethod]
        public void CheckInitVisualValid()
        {
            Assert.AreEqual(true, TestValidator1.IsVisualValid);
        }

        // после ввода невалидного значения, IsVisualValid должен измениться на false
        [TestMethod]
        public void CheckVisualValidFalse()
        {
            TestValidator1.Value = -1;
            Assert.AreEqual(false, TestValidator1.IsVisualValid);
        }

        // после ввода невалидного значения, IsVisualValid должен измениться на false
        [TestMethod]
        public void CheckVisualValidTrue()
        {
            TestValidator1.Value = 1;
            Assert.AreEqual(true, TestValidator1.IsVisualValid);
        }

        // после срабатывания CheckValid(), IsVisualValid должен стать false
        // так как используется значение init: -1
        public Validator<int> TestValidator2 => Validator<int>.Build()
            .UsingValue(-1)
            .UsingRule(x => x.Value > 0, "ERROR");
        [TestMethod]
        public void CheckWillFalse()
        {
            TestValidator2.CheckValid();
            Assert.AreEqual(false, TestValidator2.IsVisualValid);
        }
    }
}
