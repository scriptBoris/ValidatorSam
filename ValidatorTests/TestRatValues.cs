using ValidatorSam;
using ValidatorTests.Supports;

namespace ValidatorTests
{
    [TestClass]
    public class TestRatValues
    {
        // “ест на указывание InitValue как изначально не валидное
        // и игнорирование правил
        public Validator<int?> UserAge => Validator<int?>.Build()
            .UsingRule(x => x >= 18, "User must be of legal age")
            .UsingRequired();
        
        [TestMethod]
        public void CheckTestWith_MockUIInput_IgnoreRules()
        {
            var inputUserAge = new MockInputValue()
            {
                Validator = UserAge,
            };

            // »митаци€ прохих данных от сервера
            UserAge.SetValueAsRat(13, RatModes.InitValue | RatModes.SkipValidation);

            // UI поле ввода не должно отображать флаг ошибки
            Assert.AreEqual(false, inputUserAge.HasError);

            // UI поле ввода не должно отображать текст ошибки
            Assert.AreEqual(null, inputUserAge.ErrorText);

            // Ќо при этом сам валидатор должен быть невалидным, так как значение
            // которое он содержит, не соответствует правилу
            Assert.AreEqual(false, UserAge.IsValid);

            // «начение должно быть таким, каким указали в крысином методе
            Assert.AreEqual(13, UserAge.Value);
        }

        // “ест на указывание InitValue как изначально не валидное,
        // игнорирование правил и препроцессоров
        public Validator<int?> UserAge2 => Validator<int?>.Build()
            .UsingRule(x => x >= 18, "User must be of legal age")
            .UsingLimitations(0, 100)
            .UsingRequired();

        [TestMethod]
        public void CheckTestWith_MockUIInput_IgnoreRulesAndPreprocessors()
        {
            var inputUserAge = new MockInputValue()
            {
                Validator = UserAge2,
            };

            // »митаци€ прохих данных от сервера 
            UserAge2.SetValueAsRat(-13, 
                RatModes.InitValue | RatModes.SkipValidation | RatModes.SkipPreprocessors);

            // UI поле ввода не должно отображать флаг ошибки
            Assert.AreEqual(false, inputUserAge.HasError);

            // UI поле ввода не должно отображать текст ошибки
            Assert.AreEqual(null, inputUserAge.ErrorText);

            // Ќо при этом сам валидатор должен быть невалидным, так как значение
            // которое он содержит, не соответствует правилу
            Assert.AreEqual(false, UserAge2.IsValid);

            // «начение должно быть таким, каким указали в крысином методе
            Assert.AreEqual(-13, UserAge2.Value);
        }
    }
}