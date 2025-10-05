using DataAccessLayer;
using SmartAblationSystem.ViewModels;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is for the Notification Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class NotificationModel
    {
        private static NotificationModel instance;
        private Physician physician;
        private Physician currentPhysician;

        /// <summary>
        /// Returns a Notification Model object instance
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static NotificationModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NotificationModel();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets a Physician
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Physician Physician
        {
            get
            {
                return physician;
            }
            set
            {
                physician = value;
            }
        }

        /// <summary>
        /// Gets or sets the current Physician
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Physician CurrentPhysician
        {
            get
            {
                return currentPhysician;
            }
            set
            {
                currentPhysician = value;
            }
        }

        /// <summary>
        /// Initializes the instance of Notification Model
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void initialize()
        {
            instance = new NotificationModel();
        }

        /// <summary>
        /// Saves the notifications to the database
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void SaveNotification()
        {
            CommonViewModel.Current.Data.DataAccess.UpdatePhysicianPreference(CurrentPhysician);
        }
    }
}