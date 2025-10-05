using SmartAblationSystem.Helpers;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for SiteSetup.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class SiteSetup : UserControl
    {
        /// <summary>
        /// Initializes SiteSetup components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public SiteSetup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when the user control is loaded
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Represents the base class for classes that contain user control event data.</param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SensorReadingMananger.AllowRemoteControl = false;
        }
    }
}