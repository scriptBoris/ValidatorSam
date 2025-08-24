using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;
using ValidatorSam.Core;
using ValidatorTests.Supports;

namespace ValidatorTests
{
    public class CityConverter : IValueRawConverter<MockCity>
    {
        public ConverterResult<MockCity> RawToValue(ReadOnlySpan<char> rawValue, ReadOnlySpan<char> oldRawValue, MockCity oldValue, Validator validator)
        {
            if (validator.Payload is not IList<MockCity> cities)
            {
                var city = new MockCity { Id = -1, CityName = rawValue.ToString() };
                return ConverterResult.Success<MockCity>(city, rawValue);
            }

            var name = rawValue.ToString();
            var match = cities.FirstOrDefault(x => string.Equals(x.CityName, name, StringComparison.InvariantCultureIgnoreCase));
            if (match != null)
            {
                return ConverterResult.Success<MockCity>(match, rawValue);
            }
            else
            {
                return ConverterResult.Error<MockCity>("Введите правильный город", null, rawValue);
            }
        }

        public string ValueToRaw(MockCity newValue, Validator validator)
        {
            return newValue.CityName;
        }
    }

    [TestClass]
    public class TestCustomConverters
    {
        private readonly MockCity[] _cities =
        [
            new MockCity { Id = 1, CityName = "New York" },
            new MockCity { Id = 2, CityName = "London" },
            new MockCity { Id = 3, CityName = "Tokyo" },
            new MockCity { Id = 4, CityName = "Paris" },
            new MockCity { Id = 5, CityName = "Dubai" },
            new MockCity { Id = 6, CityName = "Singapore" },
            new MockCity { Id = 7, CityName = "Hong Kong" },
            new MockCity { Id = 8, CityName = "Los Angeles" },
            new MockCity { Id = 9, CityName = "Rome" },
            new MockCity { Id = 10, CityName = "Barcelona" },
            new MockCity { Id = 11, CityName = "Sydney" },
            new MockCity { Id = 12, CityName = "Berlin" },
            new MockCity { Id = 13, CityName = "Toronto" },
            new MockCity { Id = 14, CityName = "Bangkok" },
            new MockCity { Id = 15, CityName = "Moscow" },
            new MockCity { Id = 16, CityName = "Istanbul" },
            new MockCity { Id = 17, CityName = "Amsterdam" },
            new MockCity { Id = 18, CityName = "San Francisco" },
            new MockCity { Id = 19, CityName = "Chicago" },
            new MockCity { Id = 20, CityName = "Seoul" },
        ];

        public Validator<MockCity> City => Validator<MockCity>.Build()
            .UsingPayload(_cities)
            .UsingConverter(new CityConverter());

        /// <summary>
        /// Тест кастомного конвертера который эмулирует
        /// поле ввода для указания города. Но напечатанный город должен 
        /// быть из списка _cities
        /// </summary>
        [TestMethod]
        public void CheckConverter()
        {
            Assert.AreEqual("", City.RawValue);
            Assert.AreEqual(null, City.Value);

            City.RawValue = "S";
            City.RawValue = "Sy";
            City.RawValue = "Syd";

            Assert.AreEqual("Syd", City.RawValue);
            Assert.AreEqual(null, City.Value);

            City.RawValue = "Sydn";
            City.RawValue = "Sydne";
            City.RawValue = "Sydney";

            Assert.AreEqual("Sydney", City.RawValue);
            Assert.AreEqual(_cities[10], City.Value);

            City.RawValue = "SydneyJ";

            Assert.AreEqual("SydneyJ", City.RawValue);
            Assert.AreEqual(null, City.Value);
        }
    }
}
