using System.Windows.Controls;
using SmartAblationSystem.ViewModels;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for CatheterDatabase.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class CatheterDatabase : UserControl
    {
        /// <summary>
        /// Interaction logic for CatheterDatabase.xaml
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>

        private CatheterDatabaseViewModel catheterDatabaseViewModel;

        /// <summary>
        /// Get catheter info from database.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public CatheterDatabase()
        {
            InitializeComponent();
            this.catheterDatabaseViewModel = this.DataContext as CatheterDatabaseViewModel;

        }

        /// <summary>
        /// Interaction logic for CatheterDatabase.xaml reloaded
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.catheterDatabaseViewModel != null)
                this.catheterDatabaseViewModel.CatheterInformation = CommonViewModel.Current.Data.DataAccess.GetAllCatheterInformation();

        }
    }
}