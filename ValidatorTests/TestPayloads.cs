using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    [DoNotParallelize]
    public class TestPayloads
    {
        public Validator<int?> UserAge => Validator<int?>.Build()
            .UsingValue(18)
            .UsingPayload("item1", 1)
            .UsingPayload("item2", 2)
            .UsingPayload("item3", 3);

        [TestMethod]
        public void CheckPayloads()
        {
            Assert.AreEqual(18, UserAge.Value);
            Assert.AreEqual("18", UserAge.RawValue);

            if (UserAge.Payload.TryGetPayload("item1", out object value1))
                Assert.AreEqual(1, value1);
            else
                Assert.Fail();

            if (UserAge.Payload.TryGetPayload("item2", out object value2))
                Assert.AreEqual(2, value2);
            else
                Assert.Fail();

            if (UserAge.Payload.TryGetPayload("item3", out object value3))
                Assert.AreEqual(3, value3);
            else
                Assert.Fail();
        }

        public Validator<int?> UserCount => Validator<int?>.Build()
            .UsingValue(2)
            .UsingPayload("item1", 1)
            .UsingPayload("item2", 2)
            .UsingPayload("item3", 3);

        private bool isAdded;
        private bool isDeleted;

        [TestMethod]
        public void Adds()
        {
            Assert.AreEqual(2, UserCount.Value);
            Assert.AreEqual("2", UserCount.RawValue);

            UserCount.Payload.PayloadAdded += Payload_PayloadAdded;
            UserCount.Payload.PayloadRemoved += Payload_PayloadRemoved;

            UserCount.Payload.Remove("item1");
            UserCount.Payload.Push("item2", 22);
            UserCount.Payload.Push("item4", 4);

            if (UserCount.Payload.TryGetPayload("item1", out object value1))
                Assert.Fail();

            if (UserCount.Payload.TryGetPayload("item2", out object value2))
                Assert.AreEqual(22, value2);
            else
                Assert.Fail();

            if (UserCount.Payload.TryGetPayload("item3", out object value3))
                Assert.AreEqual(3, value3);
            else
                Assert.Fail();

            if (UserCount.Payload.TryGetPayload("item4", out object value4))
                Assert.AreEqual(4, value4);
            else
                Assert.Fail();

            Assert.IsTrue(isDeleted);
            Assert.IsTrue(isAdded);
        }

        private void Payload_PayloadRemoved(IValidator validator, IPayload payload, string key, object removed)
        {
            if (key == "item1" && (int)removed! == 1)
                isDeleted = true;
        }

        private void Payload_PayloadAdded(IValidator validator, IPayload payload, string key, object added)
        {
            isAdded = true;
        }

        [TestMethod]
        public void FailGet()
        {
            try
            {
                object payload = UserCount.Payload.GetPayload("no_exist");
                Assert.Fail();
            }
            catch (Exception)
            {
                // Если произошло исключение, значит метод работает корректно, т.к.
                // пытались НЕ безопасно получить несуществующий элемент
            }

            try
            {
                object payload = UserCount.Payload.GetPayload("");
                Assert.Fail();
            }
            catch (Exception)
            {
                // Если произошло исключение, значит метод работает корректно, т.к.
                // пытались НЕ безопасно получить несуществующий элемент
            }

            try
            {
                object payload = UserCount.Payload.GetPayload(null!);
                Assert.Fail();
            }
            catch (Exception)
            {
                // Если произошло исключение, значит метод работает корректно, т.к.
                // пытались НЕ безопасно получить несуществующий элемент
            }
        }
    }
}
