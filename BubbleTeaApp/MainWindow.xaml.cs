using System.Windows;
using BubbleTeaApp.ViewModels;

namespace BubbleTeaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
