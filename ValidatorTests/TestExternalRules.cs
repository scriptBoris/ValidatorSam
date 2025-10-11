using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;
using ValidatorSam.Core;
using ValidatorTests.Supports;

namespace ValidatorTests;

[TestClass]
public class TestExternalRules
{
    public Validator<List<MockCity>> Cities => Validator<List<MockCity>>.Build()
        .UsingValue([])
        .UsingRule(x => x.Value.Count > 0, "Cities is empty");

    [TestMethod]
    public void TestUserInput()
    {
        var controller = new TagController(Cities);
        controller.UserInput = "T";
        controller.UserInput = "To";
        controller.UserInput = "Tok";

        Assert.AreEqual(false, Cities.IsValid);
        Assert.AreEqual("Unknown city", Cities.TextError);
        Assert.AreEqual("", string.Join(',', Cities.Value.Select(x => x.Id)));

        controller.UserInput = "Toki";
        controller.UserInput = "Tokio";
        Assert.AreEqual(true, Cities.IsValid);
        Assert.AreEqual("1", string.Join(',', Cities.Value.Select(x => x.Id)));

        controller.UserInput = "";
        Assert.AreEqual(false, Cities.IsValid);
        Assert.AreEqual("Cities is empty", Cities.TextError);
        Assert.AreEqual("", string.Join(',', Cities.Value.Select(x => x.Id)));
    }

    [TestMethod]
    public void TestUserInputWithEnter()
    {
        var controller = new TagController(Cities);
        controller.UserInput = "Tokio";
        Assert.AreEqual(true, Cities.IsValid);
        Assert.AreEqual("1", string.Join(',', Cities.Value.Select(x => x.Id)));

        controller.PressEnter();

        Assert.AreEqual(true, Cities.IsValid);
        Assert.AreEqual(null, Cities.TextError);
        Assert.AreEqual("1", string.Join(',', Cities.Value.Select(x => x.Id)));
    }

    [TestMethod]
    public void TestUserInputWithEnterAndTypeAgain()
    {
        var controller = new TagController(Cities);
        controller.UserInput = "Tokio";
        Assert.AreEqual(true, Cities.IsValid);
        Assert.AreEqual("1", string.Join(',', Cities.Value.Select(x => x.Id)));

        controller.PressEnter();

        controller.UserInput = "Tokio";


        Assert.AreEqual(true, Cities.IsValid);
        Assert.AreEqual(null, Cities.TextError);
        Assert.AreEqual("1", string.Join(',', Cities.Value.Select(x => x.Id)));
    }

    private class TagController
    {
        private readonly Dictionary<string, MockCity> _city;
        private readonly Validator _validator;
        private MockCity? _typedCity;

        public TagController(Validator validator)
        {
            _city = new Dictionary<string, MockCity>(StringComparer.OrdinalIgnoreCase);
            validator.SetExternalRule(Handle);
            validator.ValueChanged += Validator_ValueChanged;
            string[] cities =
            [
                "Tokio",
                "Almata",
                "Muhosransk",
            ];
            for (int i = 0; i < cities.Length; i++) 
            {
                string c = cities[i];
                _city.Add(c, new MockCity
                {
                    Id = i + 1,
                    CityName = c,
                });
            }

            _validator = validator;
        }

        private void Validator_ValueChanged(Validator invoker, ValidatorValueChangedArgs args)
        {
            throw new InvalidOperationException("Значение не должно меняться!");
        }

        private ExternalRuleResult Handle(RuleArgs<object?> args)
        {
            return Result();
        }

        private ExternalRuleResult Result()
        {
            if (string.IsNullOrWhiteSpace(UserInput))
                return ExternalRuleResult.Success;

            if (_city.ContainsKey(UserInput))
            {
                return ExternalRuleResult.Success;
            }
            else
            {
                return ExternalRuleResult.Error("Unknown city");
            }
        }

        public string UserInput
        {
            get => _validator.RawValue;
            set
            {
                _validator.RawValue = value;
                if (_city.TryGetValue(value, out var matchCity))
                {
                    TypedCity = matchCity;
                }
                else
                {
                    TypedCity = null;
                }
                _validator.CheckValid();
            }
        }

        private MockCity? TypedCity
        {
            get => _typedCity;
            set
            {
                var old = _typedCity;
                _typedCity = value;
                
                if (old == value)
                    return;

                if (_validator.Value is IList list)
                {
                    // add
                    if (value != null)
                    {
                        if (list.Contains(value))
                            return;

                        list.Add(value);
                    }
                    // remove
                    else
                    {
                        list.Remove(old);
                    }
                }
            }
        }

        public void PressEnter()
        {
            if (string.IsNullOrEmpty(UserInput))
                return;

            var res = Result();
            if (!res.IsSuccess)
                return;

            _typedCity = null;
            UserInput = "";
        }
    }
}