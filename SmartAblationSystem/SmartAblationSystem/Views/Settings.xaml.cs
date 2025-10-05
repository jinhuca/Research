using System.Windows;
using System.Windows.Controls;
using SmartAblationSystem.ViewModels;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class Settings : UserControl
    {
        /// <summary>
        /// Initializes Settings components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_OnLoaded(object sender, RoutedEventArgs e)
        {
          ((SettingsViewModel)DataContext).HospitalName = CommonViewModel.Current.Data.DataAccess.GetHospitalName();
        }
    }
}