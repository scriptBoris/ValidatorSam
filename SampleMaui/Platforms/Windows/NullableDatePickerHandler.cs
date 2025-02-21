using Microsoft.Maui.Platform;
using SampleMaui.Controls.Atomic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMaui.Platforms.Windows
{
    public class NullableDatePickerHandler : NPicker.DatePickerHandler
    {
        public static PropertyMapper<NullableDatePicker, NullableDatePickerHandler> NullableDatePickerMapper = new(Mapper)
        {
            [nameof(NullableDatePicker.BorderColor)] = MapBorderColor
        };

        public NullableDatePickerHandler() : base(NullableDatePickerMapper)
        {
            
        }

        private static void MapBorderColor(NullableDatePickerHandler handler, NullableDatePicker picker)
        {
            handler.PlatformView.BorderBrush = picker.BorderColor.ToPlatform();
        }
    }
}
