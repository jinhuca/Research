using SmartAblationSystem.ViewModels;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for MessagePopup.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class ConnectCatheterPopup : Window
    {
        private uint PreviousVolume = CommonViewModel.Current.RequiredVolume;

        private DispatcherTimer ConnectionTimer = new DispatcherTimer();
        private bool isNotConncted = true;
        private CommonViewModel localCommonViewModel = CommonViewModel.Current;

        /// <summary>
        /// Message type enumeration. 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum MessageType
        {
            SystemMessage = 0,  // blue
            WarningMessage = 1, // yellow
            ErrorMessage = 2 // Red
        }

        /// <summary>
        /// Button type enumeration. 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum ButtonType
        {
            YesNo = 0,
            OkCancel = 1,
            Ok = 2
        }

        /// <summary>
        /// Initializes the MessagePopup components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="message">A string representing the message to display.</param>
        /// <param name="messageType">A MessageType enum represenging the message type.</param>
        /// <param name="buttonType">A ButtonType enum representing the button type to display.</param>
        /// <param name="messageTitle">A string representing the message's title.</param>
        public ConnectCatheterPopup(string message, MessageType messageType = MessageType.SystemMessage,
                            ButtonType buttonType = ButtonType.YesNo, string messageTitle = "")
        {
            InitializeComponent();

            //txtMessage.Text = message;

            if (buttonType == ButtonType.YesNo)
            {
                YesButton.Content = "OK";

            }
            else if (buttonType == ButtonType.OkCancel)
            {
                YesButton.Content = "Cancel";

            }
            else if (buttonType == ButtonType.Ok)
            {
                YesButton.Content = "OK";
        
            }

            if (messageType == MessageType.SystemMessage)
            {
                TitleLabel.Content = "SYSTEM MESSAGE";
                TitleLabel.Foreground = new SolidColorBrush(Colors.White);
                VolumeControlStackPanel.Visibility = Visibility.Hidden;
            }
            else if (messageType == MessageType.WarningMessage)
            {
                TitleLabel.Content = "WARNING MESSAGE";
                TitleLabel.Foreground = new SolidColorBrush(Colors.Black);
                VolumeControlStackPanel.Visibility = Visibility.Hidden;
                BorderColor.Background = (Brush)Application.Current.Resources["CryterionPopupWarningMessageBrushBackground"];
            }
            else if (messageType == MessageType.ErrorMessage)
            {
                TitleLabel.Content = "SYSTEM NOTIFICATION";
                TitleLabel.Foreground = new SolidColorBrush(Colors.White);
                VolumeControlStackPanel.Visibility = Visibility.Visible;
            }

            if (messageTitle != string.Empty)
            {
                TitleLabel.Content = messageTitle;
            }
            MuteButton.IsChecked = false;
            ConnectionTimer.Interval = TimeSpan.FromMilliseconds(1000);
  
            ConnectionTimer.Tick += new EventHandler(ConnectionTimer_tick);
            ConnectionTimer.Start();
        }
        /// <summary>
        /// Occurs when the ConnectionTimer_ticky event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void ConnectionTimer_tick(object sender, EventArgs e)
        {
            if (isNotConncted)
            { 
                    connectorOff.Visibility = Visibility.Visible;
                    connectorOn.Visibility = Visibility.Hidden;
                    isNotConncted = false;
             }

            else
            {
                connectorOff.Visibility = Visibility.Hidden;
                connectorOn.Visibility = Visibility.Visible;
                isNotConncted = true;
            }


            localCommonViewModel.ResetCanOneStopWatch();

        }

        /// <summary>
        /// Occurs when the Yes_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectionTimer.Stop();

                System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;

                RequiredVolume = PreviousVolume;

                DialogResult = true;
                this.Close();
    
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();
            }
        }

        /// <summary>
        /// Occurs when the No_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void No_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectionTimer.Stop();
                DialogResult = false;
                this.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

    /// <summary>
    /// Occurs when the MuteVolume event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Contains state information and event data associated with a routed event.</param>
    private void ToggleMuteButtonCommand(object sender, RoutedEventArgs e)
    {
      try
      {
        if (MuteButton != null)
        {
          RequiredVolume = (bool)MuteButton.IsChecked ? 0 : PreviousVolume;
        }
      }
      catch (Exception ex)
      {
        // Log the exception or handle it appropriately
        ex.ToString();
      }
    }
    /// <summary>
    /// Gets or sets the Required Volume value.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public uint RequiredVolume
        {
            get
            {
                return CommonViewModel.Current.RequiredVolume;
            }
            set
            {
                CommonViewModel.Current.RequiredVolume = value;
            }
        }

  }
}