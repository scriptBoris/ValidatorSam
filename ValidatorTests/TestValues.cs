using ValidatorSam;

namespace ValidatorTests
{
    [TestClass]
    public class TestValues
    {
        // Если пользователь указал "-" (float 0-100)
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

        // Если пользователь указал "-" (uint)
        public Validator<uint> Uint => Validator<uint>.Build()
            .UsingRequired();

        [TestMethod]
        public void UintWithMinus()
        {
            Uint.RawValue = "-";
            Assert.AreEqual(0u, Uint.Value, $"\n{Uint.TextError}");
            Assert.AreEqual("0", Uint.RawValue, $"\n{Uint.TextError}");
        }

        // Внешне указано: -20 (int? 0-322)
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

        // Строка не пустая, внешне указанно: "Boris" (3-40)
        public Validator<string> FirstName => Validator<string>.Build()
            .UsingSafeTextLimit(3, 40)
            .UsingRequired();

        [TestMethod]
        public void RequiredFirstName()
        {
            FirstName.Value = "Boris";
            Assert.AreEqual(true, FirstName.IsValid, $"\n{FirstName.TextError}");
        }

        // Строка не пустая, внешне указанно: "A" (3-40)
        public Validator<string> LastName => Validator<string>.Build()
            .UsingSafeTextLimit(3, 40)
            .UsingRequired();

        [TestMethod]
        public void RequiredLastName()
        {
            LastName.Value = "A";
            Assert.AreEqual(false, LastName.IsValid, $"\n{LastName.TextError}");
        }
    }
}