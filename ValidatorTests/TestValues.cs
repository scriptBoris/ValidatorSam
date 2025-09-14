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

        // Если пользователь указал "-" (uint 0-4_294_967_295)
        public Validator<uint> Uint => Validator<uint>.Build()
            .UsingRequired();

        [TestMethod]
        public void UintWithMinus()
        {
            Uint.RawValue = "-";
            Assert.AreEqual(0u, Uint.Value, $"\n{Uint.TextError}");
            Assert.AreEqual("", Uint.RawValue, $"\n{Uint.TextError}");
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
            .UsingTextLimit(3, 40)
            .UsingRequired();

        [TestMethod]
        public void RequiredFirstName()
        {
            FirstName.Value = "Boris";
            Assert.AreEqual(true, FirstName.IsValid, $"\n{FirstName.TextError}");
        }

        // Строка не пустая, внешне указанно: "A" (3-40)
        public Validator<string> LastName => Validator<string>.Build()
            .UsingTextLimit(3, 40)
            .UsingRequired();

        [TestMethod]
        public void RequiredLastName()
        {
            LastName.Value = "A";
            Assert.AreEqual(false, LastName.IsValid, $"\n{LastName.TextError}");
        }

        // Nullable DateTime должен быть пустой
        public Validator<DateTime?> UserBirthday => Validator<DateTime?>.Build();

        [TestMethod]
        public void TestNullableDateTimeEmpty()
        {
            Assert.AreEqual(false, UserBirthday.HasValue);
        }

        // Nullable DateTime должен быть пустой (вариант с required)
        public Validator<DateTime?> UserBirthday2 => Validator<DateTime?>.Build()
            .UsingRequired();

        [TestMethod]
        public void TestNullableDateTimeEmpty2()
        {
            Assert.AreEqual(false, UserBirthday2.HasValue);
        }

        // Тест на работоспособность перерисовки RawValue для UI
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

        public Validator<string> Comment => Validator<string>.Build();

        // Тест на эквивалентность работы Value & RawValue
        [TestMethod]
        public void TestValueAndRaw()
        {
            Comment.RawValue = "H";
            Assert.AreEqual("H", Comment.RawValue);
            Assert.AreEqual("H", Comment.Value);
            Assert.IsTrue(Comment.IsValid);
        }

        // Тест на эквивалентность работы Value & RawValue
        [TestMethod]
        public void TestValueAndRaw2()
        {
            Comment.Value = "H";
            Assert.AreEqual("H", Comment.RawValue);
            Assert.AreEqual("H", Comment.Value);
            Assert.IsTrue(Comment.IsValid);
        }
    }
}