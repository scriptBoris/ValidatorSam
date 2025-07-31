using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;
using ValidatorSam.Core;

namespace ValidatorTests
{
    [TestClass]
    public class TestAutoCastNumbers
    {
        public Validator<int> FieldInt32 => Validator<int>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckRawInt32()
        {
            FieldInt32.RawValue = "1";
            Assert.AreEqual(1, FieldInt32.Value);
            Assert.AreEqual("1", FieldInt32.RawValue);

            FieldInt32.RawValue = "";
            Assert.AreEqual(0, FieldInt32.Value);
            Assert.AreEqual("", FieldInt32.RawValue);

            FieldInt32.RawValue = "0";
            Assert.AreEqual(0, FieldInt32.Value);
            Assert.AreEqual("0", FieldInt32.RawValue);

            FieldInt32.RawValue = "04";
            Assert.AreEqual(4, FieldInt32.Value);
            Assert.AreEqual("4", FieldInt32.RawValue);

            FieldInt32.RawValue = "18";
            Assert.AreEqual(18, FieldInt32.Value);
            Assert.AreEqual("18", FieldInt32.RawValue);

            FieldInt32.RawValue = "-18";
            Assert.AreEqual(-18, FieldInt32.Value);
            Assert.AreEqual("-18", FieldInt32.RawValue);

            FieldInt32.RawValue = "--18";
            Assert.AreEqual(-18, FieldInt32.Value);
            Assert.AreEqual("-18", FieldInt32.RawValue);

            FieldInt32.RawValue = "-";
            Assert.AreEqual(0, FieldInt32.Value);
            Assert.AreEqual("-", FieldInt32.RawValue);

            FieldInt32.RawValue = "188";
            Assert.AreEqual(188, FieldInt32.Value);
            Assert.AreEqual("188", FieldInt32.RawValue);

            FieldInt32.RawValue = "188a";
            Assert.AreEqual(188, FieldInt32.Value);
            Assert.AreEqual("188", FieldInt32.RawValue);

            FieldInt32.RawValue = "1а88a";
            Assert.AreEqual(188, FieldInt32.Value);
            Assert.AreEqual("188", FieldInt32.RawValue);

            FieldInt32.RawValue = "222,0";
            Assert.AreEqual(222, FieldInt32.Value);
            Assert.AreEqual("222", FieldInt32.RawValue);

            FieldInt32.RawValue = "333.0";
            Assert.AreEqual(333, FieldInt32.Value);
            Assert.AreEqual("333", FieldInt32.RawValue);

            FieldInt32.RawValue = "HELLO WORLD";
            Assert.AreEqual(0, FieldInt32.Value);
            Assert.AreEqual("", FieldInt32.RawValue);
        }

        public Validator<int?> FieldNullableInt32 => Validator<int?>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckRawNullableInt32()
        {
            FieldNullableInt32.RawValue = "1";
            Assert.AreEqual(1, FieldNullableInt32.Value);
            Assert.AreEqual("1", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "";
            Assert.AreEqual(null, FieldNullableInt32.Value);
            Assert.AreEqual("", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "-";
            Assert.AreEqual(0, FieldNullableInt32.Value);
            Assert.AreEqual("-", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "18";
            Assert.AreEqual(18, FieldNullableInt32.Value);
            Assert.AreEqual("18", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "188";
            Assert.AreEqual(188, FieldNullableInt32.Value);
            Assert.AreEqual("188", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "188a";
            Assert.AreEqual(188, FieldNullableInt32.Value);
            Assert.AreEqual("188", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "1а88a";
            Assert.AreEqual(188, FieldNullableInt32.Value);
            Assert.AreEqual("188", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "222,0";
            Assert.AreEqual(222, FieldNullableInt32.Value);
            Assert.AreEqual("222", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "333.0";
            Assert.AreEqual(333, FieldNullableInt32.Value);
            Assert.AreEqual("333", FieldNullableInt32.RawValue);

            FieldNullableInt32.RawValue = "HELLO WORLD";
            Assert.AreEqual(null, FieldNullableInt32.Value);
            Assert.AreEqual("", FieldNullableInt32.RawValue);
        }

        public Validator<uint> FieldUint32 => Validator<uint>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckRawUInt32()
        {
            FieldUint32.RawValue = "1";
            Assert.AreEqual(1u, FieldUint32.Value);
            Assert.AreEqual("1", FieldUint32.RawValue);

            FieldUint32.RawValue = "";
            Assert.AreEqual(0u, FieldUint32.Value);
            Assert.AreEqual("", FieldUint32.RawValue);

            FieldUint32.RawValue = "-";
            Assert.AreEqual(0u, FieldUint32.Value);
            Assert.AreEqual("", FieldUint32.RawValue);

            FieldUint32.RawValue = "18";
            Assert.AreEqual(18u, FieldUint32.Value);
            Assert.AreEqual("18", FieldUint32.RawValue);

            FieldUint32.RawValue = "188";
            Assert.AreEqual(188u, FieldUint32.Value);
            Assert.AreEqual("188", FieldUint32.RawValue);

            FieldUint32.RawValue = "188a";
            Assert.AreEqual(188u, FieldUint32.Value);
            Assert.AreEqual("188", FieldUint32.RawValue);

            FieldUint32.RawValue = "1а88a";
            Assert.AreEqual(188u, FieldUint32.Value);
            Assert.AreEqual("188", FieldUint32.RawValue);

            FieldUint32.RawValue = "222,0";
            Assert.AreEqual(222u, FieldUint32.Value);
            Assert.AreEqual("222", FieldUint32.RawValue);

            FieldUint32.RawValue = "333.0";
            Assert.AreEqual(333u, FieldUint32.Value);
            Assert.AreEqual("333", FieldUint32.RawValue);

            FieldUint32.RawValue = "HELLO WORLD";
            Assert.AreEqual(0u, FieldUint32.Value);
            Assert.AreEqual("", FieldUint32.RawValue);
        }

        public Validator<double> FieldDouble => Validator<double>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckRawDouble()
        {
            FieldDouble.RawValue = "1";
            Assert.AreEqual(1.0, FieldDouble.Value);
            Assert.AreEqual("1", FieldDouble.RawValue);

            FieldDouble.RawValue = "1.0";
            Assert.AreEqual(1.0, FieldDouble.Value);
            Assert.AreEqual("1.0", FieldDouble.RawValue);

            FieldDouble.RawValue = "-";
            Assert.AreEqual(0, FieldDouble.Value);
            Assert.AreEqual("-", FieldDouble.RawValue);

            FieldDouble.RawValue = "-1.0";
            Assert.AreEqual(-1, FieldDouble.Value);
            Assert.AreEqual("-1.0", FieldDouble.RawValue);

            FieldDouble.RawValue = "18";
            Assert.AreEqual(18, FieldDouble.Value);
            Assert.AreEqual("18", FieldDouble.RawValue);

            FieldDouble.RawValue = "188";
            Assert.AreEqual(188, FieldDouble.Value);
            Assert.AreEqual("188", FieldDouble.RawValue);

            FieldDouble.RawValue = "188a";
            Assert.AreEqual(188, FieldDouble.Value);
            Assert.AreEqual("188", FieldDouble.RawValue);

            FieldDouble.RawValue = "HELLO WORLD";
            Assert.AreEqual(0, FieldDouble.Value);
            Assert.AreEqual("", FieldDouble.RawValue);

            FieldDouble.Value = 3.22;
            string? raw = FieldDouble.RawValue?.Replace(',', '.');
            Assert.AreEqual(3.22, FieldDouble.Value);
            Assert.AreEqual("3.22", raw);
        }

        public Validator<int> FieldInt32ForLimit => Validator<int>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckInt32Limit()
        {
            // check over max
            FieldInt32ForLimit.RawValue = "1234567890123456789";
            Assert.AreEqual(Int32.MaxValue, FieldInt32ForLimit.Value);
            Assert.AreEqual(Int32.MaxValue.ToString(), FieldInt32ForLimit.RawValue);

            // check over min
            FieldInt32ForLimit.RawValue = "-1234567890123456789";
            Assert.AreEqual(Int32.MinValue, FieldInt32ForLimit.Value);
            Assert.AreEqual(Int32.MinValue.ToString(), FieldInt32ForLimit.RawValue);
        }

        public Validator<uint> FieldUInt32ForLimit => Validator<uint>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckUInt32Limit()
        {
            // check over max
            FieldUInt32ForLimit.RawValue = "1234567890123456789";
            Assert.AreEqual(UInt32.MaxValue, FieldUInt32ForLimit.Value);
            Assert.AreEqual(UInt32.MaxValue.ToString(), FieldUInt32ForLimit.RawValue);

            // check over min
            FieldUInt32ForLimit.RawValue = "-1234567890123456789";
            Assert.AreEqual(UInt32.MinValue, FieldUInt32ForLimit.Value);
            Assert.AreEqual("", FieldUInt32ForLimit.RawValue);
        }

        public Validator<decimal> FieldDecimalForLimit => Validator<decimal>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckDecimalLimit()
        {
            // check over max
            FieldDecimalForLimit.RawValue = "1234567890123456789012345678901234567890";
            Assert.AreEqual(1234567890123456789012345678m, FieldDecimalForLimit.Value);
            Assert.AreEqual("1234567890123456789012345678", FieldDecimalForLimit.RawValue);

            // check over min
            FieldDecimalForLimit.RawValue = "-1234567890123456789012345678901234567890";
            Assert.AreEqual(-1234567890123456789012345678m, FieldDecimalForLimit.Value);
            Assert.AreEqual("-1234567890123456789012345678", FieldDecimalForLimit.RawValue);

            FieldDecimalForLimit.RawValue = "-1234567890.123456789012345678901234567890";
            Assert.AreEqual(-1234567890.123456789012345678m, FieldDecimalForLimit.Value);
            Assert.AreEqual("-1234567890.123456789012345678", FieldDecimalForLimit.RawValue);
        }

        public Validator<double> FieldDoubleForLimit => Validator<double>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckDoubleLimit()
        {
            // check over max
            FieldDoubleForLimit.RawValue = "12345678901234567890";
            Assert.AreEqual(1234567890123456, FieldDoubleForLimit.Value);
            Assert.AreEqual("1234567890123456", FieldDoubleForLimit.RawValue);

            // check over min
            FieldDoubleForLimit.RawValue = "-12345678901234567890";
            Assert.AreEqual(-1234567890123456, FieldDoubleForLimit.Value);
            Assert.AreEqual("-1234567890123456", FieldDoubleForLimit.RawValue);
        }

        public Validator<float> FieldFloatForLimit => Validator<float>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckFloatLimit()
        {
            // check over max
            FieldFloatForLimit.RawValue = "12345678901234567890";
            Assert.AreEqual(1234567, FieldFloatForLimit.Value);
            Assert.AreEqual("1234567", FieldFloatForLimit.RawValue);

            // check over min
            FieldFloatForLimit.RawValue = "-12345678901234567890";
            Assert.AreEqual(-1234567, FieldFloatForLimit.Value);
            Assert.AreEqual("-1234567", FieldFloatForLimit.RawValue);
        }

        public Validator<int> Orders => Validator<int>.Build()
            .UsingRequired();
        [TestMethod]
        public void CheckInt32AsEmptyString()
        {
            Orders.RawValue = "";
            Assert.AreEqual(0, Orders.Value);
            Assert.AreEqual("", Orders.RawValue);
        }

        public Validator<int> UserOldAge => Validator<int>.Build()
            .UsingLimitations(18, 100)
            .UsingRequired();
        [TestMethod]
        public void CheckInt32WithPreprocessors()
        {
            UserOldAge.RawValue = "";
            Assert.AreEqual(18, UserOldAge.Value);
            Assert.AreEqual("18", UserOldAge.RawValue);

            UserOldAge.RawValue = "1а88a";
            Assert.AreEqual(100, UserOldAge.Value);
            Assert.AreEqual("100", UserOldAge.RawValue);
        }

        public Validator<double> FieldDoubleZero => Validator<double>.Build()
            .UsingRawValueFormat("0.00")
            .UsingRequired();
        [TestMethod]
        public void CheckFloatFormatEmpty()
        {
            Assert.AreEqual("0.00", FieldDoubleZero.RawValue);
        }

        public Validator<double> FieldDoubleWithInit => Validator<double>.Build()
            .UsingValue(3)
            .UsingRawValueFormat("0.00")
            .UsingRequired();
        [TestMethod]
        public void CheckFloatFormatWithInitValue()
        {
            Assert.AreEqual("3.00", FieldDoubleWithInit.RawValue);
        }

        public Validator<double?> FieldDouble3 => Validator<double?>.Build()
            .UsingRawValueFormat("0.00")
            .UsingRequired();
        [TestMethod]
        public void CheckFloatNullableFormat()
        {
            Assert.AreEqual("", FieldDouble3.RawValue);
        }

        public Validator<double> FieldWithCultureRu => Validator<double>.Build()
            .UsingRawValueFormat("0.00", new CultureInfo("ru-RU"))
            .UsingRequired();
        [TestMethod]
        public void CheckFormatCultureRu()
        {
            Assert.AreEqual("0,00", FieldWithCultureRu.RawValue);
        }

        public Validator<double> FieldWithCultureEn => Validator<double>.Build()
            .UsingRawValueFormat("0.00", new CultureInfo("en-US"))
            .UsingRequired();
        [TestMethod]
        public void CheckFormatCultureEn()
        {
            Assert.AreEqual("0.00", FieldWithCultureEn.RawValue);
        }
    }
}
