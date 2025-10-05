using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace SmartAblationSystem
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class SplashScreen : UserControl
    {
        /// <summary>
        /// Interaction logic for SplashScreen.xaml
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public SplashScreen()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
	        base.OnInitialized(e);
	        Task.Delay(15000).ContinueWith(_ => DisplayHardwareError());
	        CanErrorLabel.Visibility = Visibility.Hidden;
        }

        private void DisplayHardwareError()
        {
	        Dispatcher.BeginInvoke((Action)(() => { CanErrorLabel.Visibility = Visibility.Visible; }));
        }
    }
}
