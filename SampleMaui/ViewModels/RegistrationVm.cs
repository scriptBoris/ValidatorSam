using SampleMaui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ValidatorSam;

namespace SampleMaui.ViewModels
{
    public class RegistrationVm : BaseVm
    {
        public Validator<string> UserName => Validator<string>.Build()
            .UsingTextLimit(2, 15)
            .UsingRequired();

        public Validator<string> Email => Validator<string>.Build()
            .UsingRule(x => !x.Contains(' '), "Email must not contain spaces.")
            .UsingRule(x => MailAddress.TryCreate(x, out _), "Invalid email")
            .UsingRequired();

        public Validator<DateTime?> BirthDate => Validator<DateTime?>.Build()
            .UsingSafeRule(x => 
            {
                var span = DateTime.Now - x;
                var date = new DateTime(span.Ticks);
                return date.Year > 18;
            }, "User must be of legal age.")
            .UsingLimitations(DateTime.MinValue, DateTime.Now);

        public Validator<int?> AgeWorkExperience => Validator<int?>.Build()
            .UsingSafeRule(x => x >= 0, "Please do not enter negative numbers")
            .UsingRequired();

        public Validator<float?> BodyWeight => Validator<float?>.Build()
            .UsingLimitations(0, 1000);

        public Validator<string> Password => Validator<string>.Build()
            .UsingRule(x => !x.Contains(' '), "Password must not contain spaces.")
            .UsingRule(x => x == PasswordConfirm.Value || !PasswordConfirm.HasValue, "Passwords do not match")
            .UsingValueChangeListener(x =>
            {
                PasswordConfirm.TryToRemoveError();
            })
            .UsingRequired();

        public Validator<string> PasswordConfirm => Validator<string>.Build()
            .UsingRule(x => !x.Contains(' '), "Password must not contain spaces.")
            .UsingRule(x => x == Password.Value, "Passwords do not match")
            .UsingValueChangeListener(x =>
            {
                Password.TryToRemoveError();
            })
            .UsingRequired();

        public Validator<bool> IsLicenseAgreement => Validator<bool>.Build()
            .UsingRequired();

        public ValidatorGroup All => ValidatorGroup.Build()
            .Include(UserName)
            .Include(Email)
            .Include(BirthDate)
            .Include(AgeWorkExperience)
            .Include(BodyWeight)
            .Include(Password)
            .Include(PasswordConfirm)
            .Include(IsLicenseAgreement);

        public ICommand CommandRegister => new Command(() =>
        {
            var fields = Validator.GetAll(this);

            if (!fields.TryCheckSuccess(out var invalids))
            {
                string errors = string.Join('\n', invalids.Select(x => $"{x.Name}: {x.TextError}"));
                DisplayMessage(errors, "Error");
            }
            else
            {
                string values = String.Join('\n', fields.Select(x => $"{x.Name}: {x.Value}"));
                DisplayMessage(values, "Success");
            }
        });
    }
}