using SampleWpf.ViewModels;
using SampleWpf.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ValidatorSam;
using WpfTest.Core;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Lab(object sender, RoutedEventArgs e)
        {
            var window = new LabWindow();
            window.Owner = this;
            window.DataContext = new LabViewModel();
            window.Show();
        }

        private void Button_Lab_initValue(object sender, RoutedEventArgs e)
        {
            var window = new LabWindow();
            window.Owner = this;
            window.DataContext = new LabViewModel("Lorem Ipsum is simply dummy text of the printing and typesetting industry");
            window.Show();
        }

        private void Button_Login(object sender, RoutedEventArgs e)
        {
            var window = new LoginWindow();
            window.Owner = this;
            window.DataContext = new LoginViewModel();
            window.Show();
        }

        private void Button_BadData(object sender, RoutedEventArgs e)
        {
            var window = new BadDataWindow();
            window.Owner = this;
            window.DataContext = new BadDataViewModel();
            window.Show();
        }
    }
}
