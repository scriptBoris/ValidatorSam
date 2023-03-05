using SampleMaui.Core;
using SampleMaui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SampleMaui.ViewModels
{
    public class MainVm : BaseVm
    {
        public ICommand CommandRegistration => new Command(() =>
        {
            var reg = new RegistrationView();
            GoTo(reg);
        });
    }
}
