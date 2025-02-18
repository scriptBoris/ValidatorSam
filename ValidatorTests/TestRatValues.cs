using ValidatorSam;
using ValidatorTests.Supports;

namespace ValidatorTests
{
    [TestClass]
    public class TestRatValues
    {
        // ���� �� ���������� InitValue ��� ���������� �� ��������
        // � ������������� ������
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

            // �������� ������ ������ �� �������
            UserAge.SetValueAsRat(13, RatModes.InitValue | RatModes.SkipValidation);

            // UI ���� ����� �� ������ ���������� ���� ������
            Assert.AreEqual(false, inputUserAge.HasError);

            // UI ���� ����� �� ������ ���������� ����� ������
            Assert.AreEqual(null, inputUserAge.ErrorText);

            // �� ��� ���� ��� ��������� ������ ���� ����������, ��� ��� ��������
            // ������� �� ��������, �� ������������� �������
            Assert.AreEqual(false, UserAge.IsValid);

            // �������� ������ ���� �����, ����� ������� � �������� ������
            Assert.AreEqual(13, UserAge.Value);
        }

        // ���� �� ���������� InitValue ��� ���������� �� ��������,
        // ������������� ������ � ��������������
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

            // �������� ������ ������ �� ������� 
            UserAge2.SetValueAsRat(-13, 
                RatModes.InitValue | RatModes.SkipValidation | RatModes.SkipPreprocessors);

            // UI ���� ����� �� ������ ���������� ���� ������
            Assert.AreEqual(false, inputUserAge.HasError);

            // UI ���� ����� �� ������ ���������� ����� ������
            Assert.AreEqual(null, inputUserAge.ErrorText);

            // �� ��� ���� ��� ��������� ������ ���� ����������, ��� ��� ��������
            // ������� �� ��������, �� ������������� �������
            Assert.AreEqual(false, UserAge2.IsValid);

            // �������� ������ ���� �����, ����� ������� � �������� ������
            Assert.AreEqual(-13, UserAge2.Value);
        }
    }
}