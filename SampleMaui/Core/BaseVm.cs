using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMaui.Core
{
    public abstract class BaseVm : BaseNotify
    {
        protected void GoTo(Page page)
        {
            App.Current.MainPage.Navigation.PushAsync(page);
        }

        protected void DisplayMessage(string msg)
        {
            App.Current.MainPage.DisplayAlert("Message", msg, "OK");
        }
    }
}
