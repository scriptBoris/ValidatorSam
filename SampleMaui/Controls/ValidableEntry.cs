using SampleMaui.Controls.Atomic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidatorSam;

namespace SampleMaui.Controls
{
    public class ValidableEntry : BorderEntry
    {
        public static readonly BindableProperty ValidatorProperty = BindableProperty.Create(
            nameof(Validator),
            typeof(Validator),
            typeof(ValidableEntry),
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is not ValidableEntry self)
                    return;

                if (o is Validator old)
                {
                    old.ErrorChanged -= self.Nev_ErrorChanged;
                }

                if (n is Validator nev)
                {
                    nev.ErrorChanged += self.Nev_ErrorChanged;
                }
            }
        );
        public Validator? Validator
        {
            get => GetValue(ValidatorProperty) as Validator;
            set => SetValue(ValidatorProperty, value);
        }

        private void Nev_ErrorChanged(Validator invoker, ValidatorErrorTextArgs args)
        {
            if (args.IsShow)
            {
                BorderColor = Colors.Red;
            }
            else
            {
                BorderColor = (Color)BorderColorProperty.DefaultValue;
            }
        }
    }
}
