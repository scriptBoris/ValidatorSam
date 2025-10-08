using SampleWpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ValidatorSam;
using WpfTest.Core;

namespace SampleWpf.ViewModels
{
    public class SilentInitViewModel : BaseViewModel
    {
        public SilentInitViewModel()
        {
            StringField.SetValueAsRat("hi", RatModes.InitValue | RatModes.SkipValidation);
            IntField.SetValueAsRat(null, RatModes.InitValue | RatModes.SkipValidation);
            DoubleField.SetValueAsRat(null, RatModes.InitValue | RatModes.SkipValidation);
            BoolField.SetValueAsRat(null, RatModes.InitValue | RatModes.SkipValidation);
        }

        public Validator<string> StringField => Validator<string>.Build()
            .UsingRule((x) => x.Value.Length >= 3, "Text so short")
            .UsingRule((x) => x.Value.Length <= 15, "Text so long")
            .UsingRequired();

        public Validator<int?> IntField => Validator<int?>.Build()
            .UsingRule((x) => x.Value >= 0, "Cant be less zero")
            .UsingRule((x) => x.Value >= 18, "Cant be less 18")
            .UsingRequired();

        public Validator<double?> DoubleField => Validator<double?>.Build()
            .UsingSafeRule((x) => x.Value >= 90, "Cant be less 90")
            .UsingSafeRule((x) => x.Value <= 100, "Cant be over 100")
            .UsingRequired();

        public Validator<bool?> BoolField => Validator<bool?>.Build()
            .UsingRequired();

        public ICommand CommandCheckValid => new Command(() =>
        {
            var all = Validator.GetAll(this);
            var res = all.FirstInvalid();

            if (!res.IsValid)
            {
                MessageBox.Show($"Field {res.Name}: {res.TextError}", res.IsValid ? "Success" : "Fail");
                return;
            }

            var l = new List<string>();
            foreach (var item in all)
            {
                l.Add($"{item.Name}: {item.Value}");
            }

            MessageBox.Show($"{string.Join('\n', l)}", "Success");
        });

        public class InnerClass
        {
            public Validator<string> InnerValidator => Validator<string>.Build().UsingRequired();
            public Validator<DateTime?> BirthDate => Validator<DateTime?>.Build().UsingRequired();
        }
    }
}
