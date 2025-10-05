using DataAccessLayer;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the Catheter Database View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class CatheterDatabaseViewModel : BindableBase
    {
        private ObservableCollection<CatheterInformation> catheterInformation;

        /// <summary>
        /// This constructor initializes the Catheter Database View Model's properties and commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public CatheterDatabaseViewModel()
        {
        }

        /// <summary>
        /// This property gets/sets the Catheter Information observable collection
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ObservableCollection<CatheterInformation> CatheterInformation
        {
            get
            {
                return CommonViewModel.Current.Data.DataAccess.GetAllCatheterInformation();
            }

            set
            {
                catheterInformation = value;
                RaisePropertyChanged("CatheterInformation");
            }
        }
    }
}