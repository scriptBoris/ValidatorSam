using SampleMaui.ViewModels;

namespace SampleMaui.Views;

public partial class RegistrationView
{
	public RegistrationView()
	{
		InitializeComponent();
		BindingContext = new RegistrationVm();
	}
}