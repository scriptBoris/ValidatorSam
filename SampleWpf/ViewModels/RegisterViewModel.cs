using SampleWpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ValidatorSam;
using ValidatorSam.Core;
using WpfTest.Core;

namespace SampleWpf.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public Validator<string> Email => Validator<string>.Build()
            .UsingRule(x => MailAddress.TryCreate(x.Value, out _), "No valid email")
            .UsingRule(x => !x.Value.Contains(' '), "Don't use spaces in email")
            .UsingRequired();

        public Validator<int?> UserAge => Validator<int?>.Build()
            .UsingRule(x => x.Value >= 18, "User must be of legal age")
            .UsingLimitations(0, 100);

        public Validator<string> Login => Validator<string>.Build()
            .UsingRule(x=> !x.Value.Contains(' '), "Don't use spaces in login")
            .UsingTextLimit(0, 20)
            .UsingRequired();

        public Validator<string> Password => Validator<string>.Build()
            .UsingRule(x => !x.Value.Contains(' '), "Don't use spaces in password")
            .UsingRequired();

        public ICommand CommandLogin => new Command(() =>
        {
            // And you will invoke this code for get validator result and invalid fields
            if (!Validator.GetAll(this).TryCheckSuccess(out var invalids))
            {
                var errors = invalids.Select(x => $"Field \"{x.Name}\": {x.TextError}");
                string msg = string.Join('\n', errors);
                MessageBox.Show(msg, "Fail");
                return;
            }

            var model = new
            {
                Email = Email.Value,
                UserAge = UserAge.Value,
                Login = Login.Value,
                Password = Password.Value,
            };
            string json = JsonSerializer.Serialize(model);
            MessageBox.Show($"You have successfully register!\n" + json);
        });
    }
}
