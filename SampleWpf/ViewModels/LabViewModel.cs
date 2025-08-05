using SampleWpf.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class LabViewModel : BaseViewModel
    {
        public LabViewModel()
        {
        }

        public LabViewModel(string stringField)
        {
            StringField.Value = stringField;
        }

        public int MaxName { get; set; } = 20;

        public Validator<string> StringLimitedField => Validator<string>.Build()
            .UsingTextLimit(3, 10)
            .UsingRequired();

        public Validator<string> StringField => Validator<string>.Build()
            .UsingRule((x) => x.Length >= 3, "Text so short")
            .UsingRule((x) => x.Length < 15, "Text so long")
            .UsingValueChangeListener(x =>
            {
                Debug.WriteLine($"UsingValueChangeListener: newvalue={x.NewValue}");
            })
            .UsingRequired();

        public Validator<int?> IntField => Validator<int?>.Build()
            .UsingRule((x) => x >= 0, "Cant be less zero")
            .UsingRule((x) => x >= 18, "Cant be less 18")
            .UsingRequired();

        public Validator<double?> DoubleField => Validator<double?>.Build()
            .UsingSafeRule((x) => x >= 90, "Cant be less 90")
            .UsingSafeRule((x) => x <= 100, "Cant be over 100")
            .UsingRequired();

        public Validator<bool> BoolField => Validator<bool>.Build()
            .UsingRequired();

        public Validator<DateTime> DateField => Validator<DateTime>.Build()
            .UsingRawValueFormat("yyyy.MM.dd")
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
