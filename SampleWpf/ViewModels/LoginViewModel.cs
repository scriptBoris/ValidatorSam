using SampleWpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ValidatorSam;
using ValidatorSam.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SampleWpf.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private object? _email;
        private object? _password;

        public Validator<string> Email => ValidatorBuilder<string>.Build(ref _email)
            .UsingRule(x => !MailAddress.TryCreate(x, out MailAddress? m), "No valid email")
            .UsingRequired();

        public Validator<string> Password => ValidatorBuilder<string>.Build(ref _password)
            .UsingRequired();
    }
}
