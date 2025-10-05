//using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace SystemUpdate
{
    /// <summary>
    /// Interaction logic for LoginUpdate.xaml
    /// </summary>
    public partial class LoginUpdate : Window
    {

        private int cryUserCode;
        private bool loginSuccess = false;
        private bool cancelPressed = false;

        // Initialize Login Window
        public LoginUpdate()
        {
            InitializeComponent();

            GeneratePasscode();
            HidePasscode();
            lblPasscode.Content = cryUserCode.ToString();
            txtUsername.Focus();
        }


        public int CryUserCode
        {
            get
            {
                return cryUserCode;
            }
            set
            {
                cryUserCode = value;
            }
        }

        // Randomly generate a passcode
        public void GeneratePasscode()
        {
            Random rnd = new Random();
            CryUserCode = rnd.Next(10000000, 99999999);
        }

        // Verify Password entered
        public bool VerifyLogin(string user)
        {
            if (user == "BSCADMIN")
            {
                string stringPasscode = CryUserCode.ToString();

                double formulaResult = Math.Pow(
                                    Int32.Parse(stringPasscode.Substring(0, 1)) +
                                    Math.Pow(Int32.Parse(stringPasscode.Substring(1, 1)), 2) +
                                    Int32.Parse(stringPasscode.Substring(2, 1)) +
                                    Int32.Parse(stringPasscode.Substring(3, 1)) * 4 +
                                    Math.Pow(Int32.Parse(stringPasscode.Substring(4, 1)), 3) +
                                    Int32.Parse(stringPasscode.Substring(5, 1)) * 5 +
                                    Math.Pow(Int32.Parse(stringPasscode.Substring(6, 1)), 2) +
                                    Int32.Parse(stringPasscode.Substring(7, 1))
                                    , 2);
                return txtPassword.Password.ToString() == formulaResult.ToString();

            }
            else if (user == "BSC")
            {
                string stringPasscode = CryUserCode.ToString();

                double formulaResult = Math.Pow(
                                    Int32.Parse(stringPasscode.Substring(0, 1)) * 2 +
                                    Math.Pow(Int32.Parse(stringPasscode.Substring(1, 1)), 2) +
                                    Int32.Parse(stringPasscode.Substring(2, 1)) +
                                    Int32.Parse(stringPasscode.Substring(3, 1)) * 3 +
                                    Int32.Parse(stringPasscode.Substring(4, 1)) +
                                    Int32.Parse(stringPasscode.Substring(5, 1)) * 4 +
                                    Math.Pow(Int32.Parse(stringPasscode.Substring(6, 1)), 3) +
                                    Int32.Parse(stringPasscode.Substring(7, 1))
                                    , 3);

                return txtPassword.Password.ToString() == formulaResult.ToString();
            }
            else
            {
                return false;
            }
        }

        // Incorrect login info entered
        public void IncorrectLogin()
        {
            txtPassword.Password = "";
            txtPassword.BorderBrush = System.Windows.Media.Brushes.Red;
        }

        // Login button
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Verify that all fields are filled.

            if (txtUsername.Text == "" || txtPassword.Password == "")
            {
                //MessageBox.Show("Please fill in all the fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                lblError.Visibility = Visibility.Visible;
                lblError.Content = "Please fill in all the fields.";
                txtUsername.Focus();
            }
#if DEBUG
#else
            else if (txtUsername.Text.ToUpper() != "BSCADMIN" && txtUsername.Text.ToUpper() != "BSC")
            {
                //MessageBox.Show("Invalid username.", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                lblError.Visibility = Visibility.Visible;
                lblError.Content = "Please enter a valid username.";
                txtUsername.BorderBrush = System.Windows.Media.Brushes.Red;
            }
#endif
            // Verify that username BSCADMIN is entered.
            // Verify that Console Serial Number entered has a valid format.
            // Verify password.
#if DEBUG
#else
            else if (!VerifyLogin(txtUsername.Text.ToUpper()))
            {
                //MessageBox.Show("Invalid password.", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                lblError.Visibility = Visibility.Visible;
                lblError.Content = "Please enter a valid password.";
                txtPassword.Password = "";
                txtPassword.BorderBrush = System.Windows.Media.Brushes.Red;
            }
#endif
            else
            {
                //System.Console.Out.WriteLine("Close");
                //KillAllOtherProcesses();
                //ZipFile ziping = new ZipFile();
                //ziping.Password = "1IPv7lBeKBNcYSQEHkONPM$JZM@a9c";
                //ziping.AddDirectory(@"C:\Users\aboujaj\Desktop\Cryterion_Medical\branches\CryoTherapyV3_dev\SystemUpdate\bin\Release");
                //ziping.Save("SystemUpdater.zip");
                this.Hide();
                //MainWindow IS FOR PHASE 2
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                loginSuccess = true;
                //DemoScreen IS FOR PHASE 1
                //DemoScreen demoScreen = new DemoScreen();
                //demoScreen.Show();
                this.Close();
            }
        }
        private void KillAllOtherProcesses()
        {
            Process current = Process.GetCurrentProcess();
            // get all the processes with current process name
            Process[] processes = Process.GetProcessesByName("SmartAblationSystem");

            foreach (Process process in processes)
            {
                //Ignore the current process  
                if (process.Id != current.Id)
                {
                    process.Kill();
                }
            }
        }

        // Cancel button
        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            MessagePopup confirmMessage = new MessagePopup("Cancelling the login process will cause the SMARTFREEZE system to reboot. Continue?");
            if ((bool)confirmMessage.ShowDialog())
            {
                cancelPressed = true;
                Close();
            }


        }

        private void DisplayPasscode()
        {
            lblPasscodeLabel.Visibility = Visibility.Visible;
            lblPasscode.Visibility = Visibility.Visible;
        }

        private void HidePasscode()
        {
            lblPasscodeLabel.Visibility = Visibility.Hidden;
            lblPasscode.Visibility = Visibility.Hidden;
        }

        private void Username_Changed(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Hidden;
            lblError.Content = "";
            txtUsername.BorderBrush = System.Windows.Media.Brushes.Black;
            if (txtUsername.Text.ToUpper() == "BSCADMIN" || txtUsername.Text.ToUpper() == "BSC")
            {
                DisplayPasscode();
            }
            else
            {
                HidePasscode();
            }
        }

        private void Password_Changed(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != "")
            {
                lblError.Visibility = Visibility.Hidden;
                lblError.Content = "";

            }
            txtPassword.BorderBrush = System.Windows.Media.Brushes.Black;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!loginSuccess)
            {
                MessagePopup confirmMessage = new MessagePopup("Cancelling the update process will cause the SMARTFREEZE system to reboot. Continue?");

                if (cancelPressed)
                {
                    System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
                }
                else
                {
                    if ((bool)confirmMessage.ShowDialog())
                    {
                        System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
                    }
                    else
                    {
                        e.Cancel = true;
                    }

                }
            }
        }
    }
}
