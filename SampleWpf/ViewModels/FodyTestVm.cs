using SampleWpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ValidatorSam;
using ValidatorSam.Core;
using WpfTest.Core;

namespace SampleWpf.ViewModels
{
    public class FodyTestViewModel: BaseViewModel
    {
        private Validator<string>? test;

        public FodyTestViewModel()
        {
            Email.RawValue = "Boris@gmail.com";

            if (Email.RawValue == "")
            {

            }
        }

        public Validator<string> Test
        {
            get
            {
                if (test == null)
                    test = TEST_BUILDER();

                return test;
            }
        }

        public Validator<string> Email => Validator<string>
            .Build()
            .UsingRule(x => !MailAddress.TryCreate(x, out MailAddress? m), "No valid email")
            .UsingRequired();

        public Validator<string> Password => Validator<string>
            .Build()
            .UsingRequired();

        public ICommand CommandLogin => new Command(() =>
        {
            var validators = Validator.GetAll(this);
            var res = validators.FirstInvalid();
            if (!res.IsValid)
            {
                MessageBox.Show($"Field {res.Name}: {res.TextError}", res.IsValid ? "Success" : "Fail");
            }
        });



        private Validator<string> TEST_RESOLVER()
        {
            if (test == null)
                test = TEST_BUILDER();
            return test;
        }

        private Validator<string> TEST_BUILDER()
        {
            return Validator<string>
                .Build()
                .UsingRule(x => !MailAddress.TryCreate(x, out MailAddress? m), "No valid email")
                .UsingRequired();
        }
    }
}
