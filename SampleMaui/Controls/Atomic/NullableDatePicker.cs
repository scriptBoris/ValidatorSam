using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMaui.Controls.Atomic;

public class NullableDatePicker : NPicker.DatePicker
{
    public NullableDatePicker()
    {
        DateSelected += NullableDatePicker_DateSelected;
    }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(NullableDatePicker),
        Colors.Gray
    );
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    private void NullableDatePicker_DateSelected(object? sender, NPicker.DateChangedEventArguments e)
    {
        this.Dispatcher.Dispatch(() =>
        {
            if (e.NewDate != null)
            {
                var d = e.NewDate.Value;
                Date = new DateTime(d.Year, d.Month, d.Day);
            }
            else
            {
                Date = null;
            }
        });
    }

    public new static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date),
        typeof(DateTime?),
        typeof(NullableDatePicker),
        null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) =>
        {
            if (b is not NPicker.DatePicker self)
                return;

            if (n is DateTime dt)
                self.Date = new DateOnly(dt.Year, dt.Month, dt.Day);
            else
                self.Date = null;
        }
    );
    public new DateTime? Date
    {
        get => GetValue(DateProperty) as DateTime?;
        set => SetValue(DateProperty, value);
    }
}