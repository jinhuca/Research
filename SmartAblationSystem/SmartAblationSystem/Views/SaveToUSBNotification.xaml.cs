using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for SaveToUSBNotification.xaml
    /// </summary>
    public partial class SaveToUSBNotification : Window
    {
     
        public SaveToUSBNotification()
        {
            
            InitializeComponent();
        }
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
       
        }
    }
}
