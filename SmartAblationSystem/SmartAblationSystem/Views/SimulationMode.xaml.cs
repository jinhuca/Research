using Communication;
using System.Threading;
using System.Threading.Tasks;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for SimulationMode.xaml
    /// </summary>
    public partial class SimulationMode : Window
    {
        public SimulationMode(object dataContext)
        {
            InitializeComponent();
        }

        private void SimulationModeTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}