using SampleMaui.Views;

namespace SampleMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

#if WINDOWS
        protected override Window CreateWindow(IActivationState activationState)
        {
            var w = base.CreateWindow(activationState);
            w.Width = 500;
            w.Height = 700;
            return w;
        }
#endif
    }
}