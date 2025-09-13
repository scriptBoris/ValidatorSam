using System;
using System.Collections.Generic;
using System.Text;
using ValidatorSam.Core;

#pragma warning disable CS1591
namespace ValidatorSam.Localizations
{
    public class Localization_EN : ValidatorLocalization
    {
        public override string StringLengthLess => "The value cannot be shorter than {0} characters";
        public override string StringLengthOver => "The value cannot be longer than {0} characters";
        public override string StringValueLess => "The value cannot be less than {0}";
        public override string StringValueOver => "The value cannot be greater than {0}";
        public override string StringRequired => "Required";

        public override string StringInvalidInputForDateTime => "Please enter the date in the format";
        public override string StringMonthIsOverflow => "The entered month cannot contain more than {0} days";
        public override string StringInvalidYear => "Invalid year";
        public override string StringInvalidMonth => "Invalid month";
        public override string StringInvalidDay => "Invalid day";
        public override string StringInvalidHour => "Invalid hour";
        public override string StringInvalidMinute => "Invalid minute";
        public override string StringInvalidSecond => "Invalid second";
    }
}
#pragma warning restore CS1591