using SampleMaui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ValidatorSam;

namespace SampleMaui.ViewModels
{
    public class RegistrationVm : BaseVm
    {
        public Validator<string> UserName => Validator<string>.Build()
            .UsingRequired();

        public Validator<string> Password => Validator<string>.Build()
            .UsingRequired();

        public Validator<int?> AgeWorkExperience => Validator<int?>.Build()
            .UsingRequired();

        public Validator<bool> IsLicenseAgreement => Validator<bool>.Build()
            .UsingRequired();

        public ICommand CommandRegister => new Command(() =>
        {
            var fields = Validator.GetAll(this);
            string values = String.Join('\n', fields.Select(x => $"{x.Name}: {x.Value}"));

            var check = fields.FirstInvalid();
            if (!check.IsValid)
            {
                DisplayMessage($"{check.Name}: {check.TextError}\n{values}");
            }
            else
            {
                DisplayMessage($"Success\n{values}");
            }
        });
    }
}
