using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

#nullable enable
namespace ValidatorSam
{
    public struct ValidatorResult
    {
        internal ValidatorResult(bool isSuccess, string? errorText, string name)
        {
            IsValid = isSuccess;
            TextError = errorText;
            Name = name;
        }

        public string? TextError { get; private set; }
        public bool IsValid { get; private set; }
        public string Name { get; private set; }
    }
}
#nullable disable