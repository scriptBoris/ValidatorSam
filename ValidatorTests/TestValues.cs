using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestValues
    {
        // ���� ������������ ������ "-" (float 0-100)
        public Validator<float> Float => Validator<float>.Build()
            .UsingLimitations(0, 100)
            .UsingRequired();
        
        [TestMethod]
        public void TestFloatMinus()
        {
            Float.RawValue = "-";
            Assert.AreEqual(0, Float.Value, $"\n{Float.TextError}");
            Assert.AreEqual("-", Float.RawValue, $"\n{Float.TextError}");
        }

        // ���� ������������ ������ "-" (uint 0-4_294_967_295)
        public Validator<uint> Uint => Validator<uint>.Build()
            .UsingRequired();

        [TestMethod]
        public void UintWithMinus()
        {
            Uint.RawValue = "-";
            Assert.AreEqual(0u, Uint.Value, $"\n{Uint.TextError}");
            Assert.AreEqual("", Uint.RawValue, $"\n{Uint.TextError}");
        }

        // ������ �������: -20 (int? 0-322)
        public Validator<int?> Count => Validator<int?>.Build()
            .UsingValue(0)
            .UsingLimitations(0, 322)
            .UsingRequired();

        [TestMethod]
        public void ValueOutFromLimitations()
        {
            Count.Value = -20;
            Assert.AreEqual(0, Count.Value, $"\n{Count.TextError}");
            Assert.AreEqual("0", Count.RawValue, $"\n{Count.TextError}");
        }

        // ������ �� ������, ������ ��������: "Boris" (3-40)
        public Validator<string> FirstName => Validator<string>.Build()
            .UsingTextLimit(3, 40)
            .UsingRequired();

        [TestMethod]
        public void RequiredFirstName()
        {
            FirstName.Value = "Boris";
            Assert.AreEqual(true, FirstName.IsValid, $"\n{FirstName.TextError}");
        }

        // ������ �� ������, ������ ��������: "A" (3-40)
        public Validator<string> LastName => Validator<string>.Build()
            .UsingTextLimit(3, 40)
            .UsingRequired();

        [TestMethod]
        public void RequiredLastName()
        {
            LastName.Value = "A";
            Assert.AreEqual(false, LastName.IsValid, $"\n{LastName.TextError}");
        }

        // Nullable DateTime ������ ���� ������
        public Validator<DateTime?> UserBirthday => Validator<DateTime?>.Build();

        [TestMethod]
        public void TestNullableDateTimeEmpty()
        {
            Assert.AreEqual(false, UserBirthday.HasValue);
        }

        // Nullable DateTime ������ ���� ������ (������� � required)
        public Validator<DateTime?> UserBirthday2 => Validator<DateTime?>.Build()
            .UsingRequired();

        [TestMethod]
        public void TestNullableDateTimeEmpty2()
        {
            Assert.AreEqual(false, UserBirthday2.HasValue);
        }

        // ���� �� ����������������� ����������� RawValue ��� UI
        public Validator<int?> OrderNumber => Validator<int?>.Build()
            .UsingRequired();
        [TestMethod]
        public void TestEmulation()
        {
            string? rawUI = null;
            OrderNumber.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "RawValue")
                    rawUI = OrderNumber.RawValue;
            };

            OrderNumber.RawValue = "123";
            Assert.AreEqual(123, OrderNumber.Value);
            Assert.AreEqual("123", rawUI);
            Assert.AreEqual(true, OrderNumber.IsValid);
        }
    }
}