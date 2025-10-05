using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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
    /// Interaction logic for DemoScreen.xaml
    /// </summary>
    public partial class DemoScreen : Window
    {
        private USBDriveConnectionManager.USBDriveConnectionManager usbDriveConnectionManager;
        public List<DriveInfo> USBDriveList;

        public DemoScreen()
        {
            InitializeComponent();
            usbDriveConnectionManager = new USBDriveConnectionManager.USBDriveConnectionManager(USBDriveConnection_EventArrived);
            try
            {
                USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();
            }
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //UpdateGui();

        }
        private void USBDriveConnection_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                USBDriveList = usbDriveConnectionManager.GetUSBDriveList();
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
        }
        //private void UpdateGui()
        //{
        //    Process GUIprocess = new Process();
        //    GUIprocess.StartInfo.FileName = "msiexec.exe";
        //    GUIprocess.StartInfo.Arguments = string.Format("/i {0} /qb+ /promptrestart", System.IO.Path.Combine(USBDriveList[0].Name,"Setup.msi"));

        //    GUIprocess.Start();
        //    GUIprocess.WaitForExit();
        //}

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            //Show popup

            MessagePopup confirmMessage = new MessagePopup("Exiting the application will cause the SMARTFREEZE system to reboot.Continue ?");
            if ((bool)confirmMessage.ShowDialog())
            {
                System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
            }
        }
    }
}
