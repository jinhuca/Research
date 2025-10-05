using DataAccessLayer;
using SmartAblationSystem.ViewModels;
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
    /// Interaction logic for DataLogPopup.xaml
    /// </summary>
    public partial class DataLogPopup : Window
    {
        ReportViewModel reportViewModel;
        TreatmentRecordsViewModel treatmentRecordsViewModel;
        /// <summary>
        /// Initializes Data Log Popup components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>

        public DataLogPopup(object dataContext)
        {
            InitializeComponent();

            if (dataContext.GetType() == typeof(TreatmentRecordsViewModel))
            {
                treatmentRecordsViewModel = dataContext as TreatmentRecordsViewModel;
                ProcedureLogsDataGrid.ItemsSource = treatmentRecordsViewModel?.ProcedureLogs;
            }
            else
            {
                reportViewModel = dataContext as ReportViewModel;
                ProcedureLogsDataGrid.ItemsSource = reportViewModel.ProcedureLogs;
            }

         

        }
        /// Occurs when the OkButton_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
