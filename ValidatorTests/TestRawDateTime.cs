using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;
using ValidatorSam.Converters;

namespace ValidatorTests
{
    [TestClass]
    public class TestRawDateTime
    {
        public Validator<DateTime> UserOldAge => Validator<DateTime>.Build()
            .UsingRawValueFormat("yyyy.MM.dd");

        public Validator<DateTime> BirthDay => Validator<DateTime>.Build()
            .UsingValue(new DateTime(1991, 1, 1))
            .UsingRawValueFormat("yyyy.MM.dd");

        [TestMethod]
        public void ParseGoodInputDate() 
        {
            UserOldAge.RawValue = "2007.1.2";
            Assert.AreEqual("2007.1.2", UserOldAge.RawValue);
            Assert.AreEqual(new DateTime(2007, 1, 2), UserOldAge.Value);
        }

        [TestMethod]
        public void ParseShortYear()
        {
            UserOldAge.RawValue = "94.1.2";
            Assert.AreEqual("94.1.2", UserOldAge.RawValue);
            Assert.AreEqual(new DateTime(1994, 1, 2), UserOldAge.Value);
            Assert.AreEqual(true, UserOldAge.IsValid);
        }

        [TestMethod]
        public void DayWith_d_Char()
        {
            UserOldAge.RawValue = "1992.12.3d";
            Assert.AreEqual("1992.12.3", UserOldAge.RawValue);
            Assert.AreEqual(new DateTime(1992,12,3), UserOldAge.Value);
            Assert.IsTrue(UserOldAge.IsValid);
        }

        [TestMethod]
        public void MonthWith_d_Char()
        {
            UserOldAge.RawValue = "1992.1d.31";
            Assert.AreEqual("1992.1.31", UserOldAge.RawValue);
            Assert.AreEqual(new DateTime(1992, 1, 31), UserOldAge.Value);
            Assert.IsTrue(UserOldAge.IsValid);
        }

        [TestMethod]
        public void StartWithDots()
        {
            UserOldAge.RawValue = ".";
            UserOldAge.RawValue = ".";
            UserOldAge.RawValue = ".";
            Assert.AreEqual("", UserOldAge.RawValue);
            Assert.AreEqual(default, UserOldAge.Value);
            Assert.IsFalse(UserOldAge.IsValid);
        }

        // Пользователь очень быстро вводит значения из numpad'а
        [TestMethod]
        public void ParseDirectRaw()
        {
            UserOldAge.RawValue = "19941213";
            Assert.AreEqual("1994.12.13", UserOldAge.RawValue);
            Assert.AreEqual(new DateTime(1994, 12, 13), UserOldAge.Value);
        }

        [TestMethod]
        public void ParseIdealInputDate()
        {
            UserOldAge.RawValue = "2007.01.02";
            Assert.AreEqual("2007.01.02", UserOldAge.RawValue);
            Assert.AreEqual(new DateTime(2007, 1, 2), UserOldAge.Value);
        }

        public Validator<DateTime> UserOldAge2 => Validator<DateTime>.Build()
            .UsingRawValueFormat("MM.dd.yyyy")
            .UsingValue(new DateTime(2001, 3, 2));

        [TestMethod]
        public void CheckInitWithFormat()
        {
            Assert.AreEqual("03.02.2001", UserOldAge2.RawValue);
            Assert.AreEqual(new DateTime(2001, 3, 2), UserOldAge2.Value);
        }

        [TestMethod]
        public void MiddleFix()
        {
            UserOldAge.RawValue = "2001.03.01";
            Assert.AreEqual("2001.03.01", UserOldAge.RawValue);
            Assert.AreEqual(new DateTime(2001, 3, 1), UserOldAge.Value);
            Assert.IsTrue(UserOldAge.IsValid);

            UserOldAge.RawValue = "2001.0.01";
            Assert.AreEqual("2001.0.01", UserOldAge.RawValue);
            Assert.AreEqual(default, UserOldAge.Value);
            Assert.IsFalse(UserOldAge.IsValid);
        }

        [TestMethod]
        public void ZeroDay()
        {
            BirthDay.RawValue = "2001.03.0";
            Assert.AreEqual("2001.03.0", BirthDay.RawValue);
            Assert.AreEqual(default, BirthDay.Value);
            Assert.IsFalse(BirthDay.IsValid);
        }

        public Validator<DateTime> BigSeparator => Validator<DateTime>.Build()
            .UsingValue(new DateTime(1991, 1, 1))
            .UsingRawValueFormat("yyyy---MM---dd");

        [TestMethod]
        public void MaskWithBigSeparators()
        {
            BigSeparator.RawValue = "2001---3---5";
            Assert.AreEqual("2001---3---5", BigSeparator.RawValue);
            Assert.AreEqual(new DateTime(2001, 3, 5), BigSeparator.Value);
            Assert.IsTrue(BigSeparator.IsValid);
        }

        public Validator<DateTime> DateWin => Validator<DateTime>.Build()
            .UsingRawValueFormat("yyyy/MM/dd HH:mm");

        [TestMethod]
        public void IdealDateWithTime()
        {
            DateWin.RawValue = "2001/11/12 12:48";
            Assert.AreEqual("2001/11/12 12:48", DateWin.RawValue);
            Assert.AreEqual(new DateTime(2001, 11, 12, 12, 48, 0), DateWin.Value);
            Assert.IsTrue(DateWin.IsValid);
        }

        [TestMethod]
        public void NoPassBadChars()
        {
            DateWin.RawValue = "ы";
            Assert.AreEqual("", DateWin.RawValue);
            Assert.AreEqual(default, DateWin.Value);
            Assert.IsFalse(DateWin.IsValid);
        }

        [TestMethod]
        public void UserForgetInputTime()
        {
            DateWin.RawValue = "2001/11/12";
            Assert.AreEqual("2001/11/12", DateWin.RawValue);
            Assert.AreEqual(default, DateWin.Value);
            Assert.IsFalse(DateWin.IsValid);
        }

        [TestMethod]
        public void GoodInputDateTimeButEmptyLongTail()
        {
            DateWin.RawValue = "2001/11/12приветмир!как дела      хорошо? Да";
            Assert.AreEqual("2001/11/12", DateWin.RawValue);
            Assert.AreEqual(default, DateWin.Value);
            Assert.IsFalse(DateWin.IsValid);
        }

        [TestMethod]
        public void BadInputLongAndLongInput()
        {
            DateWin.RawValue = "LoremImpumRandomWordAndDogsAndCats I LOVE Chikens and coca-cola. IHate frgomhed and hoboaowao";
            Assert.AreEqual("", DateWin.RawValue);
            Assert.AreEqual(default, DateWin.Value);
            Assert.IsFalse(DateWin.IsValid);
        }

        [TestMethod]
        public void StandaloneConverter()
        {
            var conv = new DateTimeConverter("dd.MM.yyyy");
            var culture = CultureInfo.InvariantCulture;
            var res = conv.MasterParse("28.11.2022", culture, "dd.MM.yyyy");

            Assert.AreEqual(new DateTime(2022, 11, 28), res.Result);
            Assert.AreEqual("28.11.2022", res.RawResult);
        }

        [TestMethod]
        public void StandaloneConverter2()
        {
            var conv = new DateTimeConverter("dd.MM.yyyy");
            var culture = CultureInfo.InvariantCulture;
            var res = conv.MasterParse("28.1.2022", culture, "dd.MM.yyyy");

            Assert.AreEqual(new DateTime(2022, 1, 28), res.Result);
            Assert.AreEqual("28.1.2022", res.RawResult);
        }

        [TestMethod]
        public void StandaloneConverter3()
        {
            var conv = new DateTimeConverter("dd.MM.yyyy");
            var culture = CultureInfo.InvariantCulture;
            var res = conv.MasterParse("28.ы1.2022", culture, "dd.MM.yyyy");

            Assert.AreEqual(new DateTime(2022, 1, 28), res.Result);
            Assert.AreEqual("28.1.2022", res.RawResult);
        }

        public Validator<DateTime?> DateArrival => Validator<DateTime?>.Build()
            .UsingRawValueFormat("dd.MM.yyyy");

        [TestMethod]
        public void TestNullDateWithFormat()
        {
            DateArrival.RawValue = "25.11.2025";
            Assert.AreEqual(new DateTime(2025, 11, 25), DateArrival.Value);
            Assert.AreEqual("25.11.2025", DateArrival.RawValue);
            Assert.IsTrue(DateArrival.IsValid);

            DateArrival.RawValue = "2";
            Assert.AreEqual(null, DateArrival.Value);
            Assert.AreEqual("2", DateArrival.RawValue);
            Assert.IsFalse(DateArrival.IsValid);

            DateArrival.RawValue = "";
            Assert.AreEqual(null, DateArrival.Value);
            Assert.AreEqual("", DateArrival.RawValue);
            Assert.IsTrue(DateArrival.IsValid);
        }
    }
}