﻿using SampleWpf.Core;
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
    public class LoginViewModel : BaseViewModel
    {
        private Validator<string>? test;

        public LoginViewModel()
        {
            Email.RawValue = "Boris@gmail.com";

            if (Email.RawValue == "")
            {

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
                MessageBox.Show($"Field name \"{res.Name}\" error: {res.TextError}", "Fail");
                return;
            }

            string email = Email;
            string pass = Password;
            string fields = $"email: \"{email}\"\npass: \"{pass}\"";
            MessageBox.Show($"You have successfully logged in\n" + fields);
        });
    }
}
