using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestEnableChanged
    {
        private bool _isUserNameEnabled = true;

        public Validator<string> UserName => Validator<string>.Build()
            .UsingRequired();

        [TestMethod]
        public void CheckDefaultError()
        {
            UserName.EnabledChanged += Test1_EnabledChanged;
            UserName.IsEnabled = false;
            Assert.AreEqual(false, _isUserNameEnabled);
        }

        private void Test1_EnabledChanged(IValidator invoker, bool args)
        {
            _isUserNameEnabled = args;
        }
    }
}