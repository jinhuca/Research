using SmartAblationSystem.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for logon.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class logonOld : Window
    {
        /// <summary>
        /// Initializes Logon components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// <param name="dataContext">An object representing the Data Context.</param>
        /// </summary>
        public logonOld(object dataContext)
        {
            InitializeComponent();

            this.DataContext = dataContext as MainWindowViewModel;
        }

        /// <summary>
        /// Occurs when the butOK_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Occurs when the butCancel_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Occurs when the User text box TextChanged event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void User_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (TxtUser.Text.ToUpper() == "BSC" || TxtUser.Text.ToUpper() == "BSCADMIN")
            {
                StackPasswordCode.Visibility = Visibility.Visible;
            }
            else
            {
                StackPasswordCode.Visibility = Visibility.Hidden;
            }
        }

        //private void ShowKeyboard(object sender, RoutedEventArgs e)
        //{
        //    TextBox tBox = sender as TextBox;
        //    TouchScreenKeyboard.SetTouchScreenKeyboard(tBox, true);
        //    //TouchScreenKeyboard.SetTouchScreenKeyboard(TxtUser, false);
        //}

        //private void ShowKeyboard_touch(object sender, TouchEventArgs e)
        //{
        //    TextBox tBox = sender as TextBox;
        //    TouchScreenKeyboard.SetTouchScreenKeyboard(tBox, true);
        //    //TouchScreenKeyboard.SetTouchScreenKeyboard(TxtUser, false);
        //}

        //private void ShowKeyboard(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    TextBox tBox = sender as TextBox;
        //    TouchScreenKeyboard.SetTouchScreenKeyboard(tBox, true);
        //}

    }
}