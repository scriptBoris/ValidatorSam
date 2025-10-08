using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;
using static System.Net.Mime.MediaTypeNames;

namespace ValidatorTests
{
    [TestClass]
    public class TestGroups
    {
        public Validator<string> Test1 => Validator<string>.Build()
            .UsingRule(x => x.Value.Length >= 2, "Error");

        public Validator<string> Test2 => Validator<string>.Build()
            .UsingRule(x => x.Value.Length >= 5, "Error2");

        public ValidatorGroup TestGroup => ValidatorGroup.Build()
            .Include(Test1)
            .Include(Test2);

        public ValidatorGroup EmptyGroup => ValidatorGroup.Build();

        // тест работу IsValid
        [TestMethod]
        public void Test_IsValid()
        {
            Test1.Value = "AB";
            Test2.Value = "ABCDE";
            Assert.AreEqual(true, TestGroup.IsValid);
            Assert.AreEqual(false, TestGroup.HasErrors);
        }

        // тест работу HasErrors
        [TestMethod]
        public void Test_HasErrors()
        {
            Test1.Value = "AB";
            Test2.Value = "AB";
            Assert.AreEqual(false, TestGroup.IsValid);
            Assert.AreEqual(true, TestGroup.HasErrors);
        }

        // пустая группа должна быть невалидной и несодержать ошибок
        [TestMethod]
        public void Test_DefaultInvalid()
        {
            Assert.AreEqual(false, EmptyGroup.IsValid);
            Assert.AreEqual(false, EmptyGroup.HasErrors);
        }

        [TestMethod]
        public void Test_EventError()
        {
            Test1.Value = "AB";
            bool isInvoked = false;
            TestGroup.ErrorsChanged += (sender, args) =>
            {
                isInvoked = true;
            };
            Test1.Value = "";
            Assert.AreEqual(true, isInvoked);
        }

        [TestMethod]
        public void Test_EventError_WithInvokerName()
        {
            Test1.Value = "AB";
            string? invokerName = null;
            TestGroup.ErrorsChanged += (sender, args) =>
            {
                invokerName = args.PropertyName;
            };
            Test1.Value = "";
            Assert.AreEqual(Test1.Name, invokerName);
        }

        [TestMethod]
        public void Test_DataErrorInfo()
        {
            Test1.Value = "AB";
            string? invokerName = null;
            TestGroup.ErrorsChanged += (sender, args) =>
            {
                invokerName = args.PropertyName;
            };
            Test1.Value = "";

            string? error = null;
            var en = ((INotifyDataErrorInfo)TestGroup).GetErrors(invokerName);

            foreach (var item in en)
            {
                error = item as string;
            }
            
            Assert.AreEqual("Error", error);
        }

        [TestMethod]
        public void Test_GroupEventError()
        {
            Test1.Value = "AB";
            string? error = null;
            TestGroup.ErrorChanged += (sender, args) =>
            {
                error = args.ErrorText;
            };
            Test1.Value = "";

            Assert.AreEqual("Error", error);
        }

        [TestMethod]
        public void Test_GroupEventError2()
        {
            Test1.Value = "";
            Test2.Value = "";

            string? error = null;
            TestGroup.ErrorChanged += (sender, args) =>
            {
                error = args.ErrorText;
            };
            Test1.Value = "small";
            Test2.Value = "A";

            Assert.AreEqual("Error2", error);
        }

        [TestMethod]
        public void Test_IsEnabled()
        {
            Test1.IsEnabled = false;
            Test2.IsEnabled = false;
            Assert.AreEqual(false, TestGroup.IsEnabled);

            Test1.IsEnabled = true;
            Test2.IsEnabled = false;
            Assert.AreEqual(true, TestGroup.IsEnabled);

            Test1.IsEnabled = true;
            Test2.IsEnabled = true;
            Assert.AreEqual(true, TestGroup.IsEnabled);

            bool? isEnabled = null;
            TestGroup.EnabledChanged += (sender, args) =>
            {
                isEnabled = args;
            };
            Test1.IsEnabled = false;
            Test2.IsEnabled = false;
            Assert.AreEqual(false, isEnabled);
            isEnabled = null;

            Test1.IsEnabled = true;
            Test2.IsEnabled = false;
            Assert.AreEqual(true, isEnabled);
            isEnabled = null;

            Test1.IsEnabled = true;
            Test2.IsEnabled = true;
            Assert.AreEqual(true, isEnabled);
            isEnabled = null;
        }
    }
}
