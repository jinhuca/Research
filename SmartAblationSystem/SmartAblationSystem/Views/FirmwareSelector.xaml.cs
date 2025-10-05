using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
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

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for FirmwareSelector.xaml
    /// </summary>
    public partial class FirmwareSelector : Window
    {
        System.Windows.Controls.DataGridCellInfo previouscell;



        /// <summary>
        /// Interaction logic for FirmwareSelector.
        /// </summary>
        public FirmwareSelector(object dataContext)
        {
            InitializeComponent();

            if (dataContext != null)
            {
                this.DataContext = dataContext;
            }
        }


        /// <summary>
        /// Occurs when the Cancel_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                this.Close();
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();
            }
        }

        /// <summary>
        /// Occurs when the OkButton_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

    }
}
