using HospitalApp.WPF.ViewModels;
using System.Windows;

namespace HospitalApp.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
