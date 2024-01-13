using SampleWpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ValidatorSam;
using WpfTest.Core;

namespace SampleWpf.ViewModels
{
    public class BadDataViewModel : BaseViewModel
    {
        public BadDataViewModel()
        {
            CountCalls.SetValueAsRat(-20, RatModes.Default | RatModes.SkipPreprocessors);
        }

        public Validator<int?> CountCalls => Validator<int?>.Build()
            .UsingLimitations(0, 322)
            .UsingRequired();

        public ICommand CommandCheckValid => new Command(() =>
        {
            var all = Validator.GetAll(this);
            var res = all.FirstInvalidOrDefault();

            if (res != null && !res.Value.IsValid)
            {
                MessageBox.Show($"Field {res.Value.Name}: {res.Value.TextError}", res.Value.IsValid ? "Success" : "Fail");
                return;
            }

            var l = new List<string>();
            foreach (var item in all)
            {
                l.Add($"{item.Name}: {item.Value}");
            }

            MessageBox.Show($"{string.Join('\n', l)}", "Success");
        });
    }
}
