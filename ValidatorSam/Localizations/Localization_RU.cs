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
    }
}
#pragma warning restore CS1591