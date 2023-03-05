using SampleMaui.ViewModels;

namespace SampleMaui.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainVm();
        }
    }
}