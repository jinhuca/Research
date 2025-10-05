using System;
using System.IO;
using System.Windows;
using System.Windows.Xps.Packaging;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// Safety classification: No injury or damage to health is possible(IEC 62304 Class A).
    /// </summary>
    public partial class UserManualWindow : Window
    {
        private XpsDocument manualDocument = null;
        private string manualLocation;

        /// <summary>
        /// Initializes UserManualWindow components.
        /// Safety classification: No injury or damage to health is possible(IEC 62304 Class A).
        /// </summary>
        public UserManualWindow()
        {
            InitializeComponent();
            try
            {
                manualLocation = Directory.GetCurrentDirectory() + @"\UserManual.xps";
                
                if(manualLocation != null)
                this.documentViewer.Document = new XpsDocument(manualLocation, FileAccess.Read).GetFixedDocumentSequence();
            }

            catch (Exception ex)
            {
                // TODO
                ex.ToString();
            }
        }

        /// <summary>
        /// Occurs when the Yes_Click event is raised.
        /// Safety classification: No injury or damage to health is possible(IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}