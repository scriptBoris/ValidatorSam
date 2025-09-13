using System;
using System.Collections.Generic;
using System.Text;
using ValidatorSam.Core;

#pragma warning disable CS1591
namespace ValidatorSam.Localizations
{
    public class Localization_RU : ValidatorLocalization
    {
        public override string StringLengthLess => "Минимум {0} символов";
        public override string StringLengthOver => "Максимум {0} символов";
        public override string StringValueLess => "Минимум {0}";
        public override string StringValueOver => "Максимум {0}";
        public override string StringRequired => "Обязательно";

        public override string StringInvalidInputForDateTime => "Введите дату по формату";
        public override string StringMonthIsOverflow => "Введенный месяц не может содержать больше (0) дней";
        public override string StringInvalidYear => "Введите валидный год";
        public override string StringInvalidMonth => "Введите валидный месяц";
        public override string StringInvalidDay => "Введите валидный день";
        public override string StringInvalidHour => "Введите валидный час";
        public override string StringInvalidMinute => "Введите валидную мунуту";
        public override string StringInvalidSecond => "Введите валидную секунду";

    }
}
#pragma warning restore CS1591