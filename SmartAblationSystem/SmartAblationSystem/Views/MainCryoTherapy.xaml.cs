using System.Windows.Controls;
using System.Windows;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for MainCryoTherapy.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class MainCryoTherapy : UserControl
    {
        /// <summary>
        /// Initializes Main Cryotherapy components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MainCryoTherapy()
        {
            InitializeComponent();

#if Simulator
            BtnStateSimulator.Visibility = Visibility.Visible;
            BtnUnknownState.Visibility = Visibility.Visible;
            BtnExceptionState.Visibility = Visibility.Visible;
            BtnTemperatureDecreaseSimulator.Visibility = Visibility.Visible;
            BtnTemperatureIncreaseSimulator.Visibility= Visibility.Visible;
            BtnError.Visibility = Visibility.Visible;
            BtnTogglePressureSensor.Visibility = Visibility.Visible;
            BtnIncreaseBloodPressure.Visibility = Visibility.Visible;
            BtnDecreaseBloodPressure.Visibility = Visibility.Visible;
            PlusSimulator.Visibility = Visibility.Visible;
            MoinsSimulator.Visibility = Visibility.Visible;
            EtsPlusSimulator.Visibility = Visibility.Visible;
            ETSMoinsSimulator.Visibility = Visibility.Visible;

#endif
        }
    }
}