using ValidatorSam;
using ValidatorTests.Supports;

namespace ValidatorTests
{
    [TestClass]
    public class TestRatValues
    {
        public TestRatValues()
        {
            input = new()
            {
                Validator = Float,
            };

            Float.SetValueAsRat(-1, RatModes.InitValue | RatModes.SkipValidation);
        }

        private MockInputValue input;

        public Validator<float?> Float => Validator<float?>.Build()
            .UsingRule(x => x < 0, "value cannot be less zero")
            .UsingRequired();
        
        [TestMethod]
        public void CheckTest()
        {
            Assert.AreEqual(false, input.HasError, $"GGG");
            Assert.AreEqual(null, input.ErrorText, $"HELLO");
        }
    }
}