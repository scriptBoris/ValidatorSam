using Microsoft.Extensions.Logging;
using NPicker;
using SampleMaui.Controls.Atomic;

#if WINDOWS
using SampleMaui.Platforms.Windows;
#endif

namespace SampleMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseNPicker()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMauiHandlers(x =>
                {
#if WINDOWS
                    x.AddHandler(typeof(BorderEntry), typeof(BorderEntryHandler));
                    x.AddHandler(typeof(NullableDatePicker), typeof(NullableDatePickerHandler));
#endif
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}