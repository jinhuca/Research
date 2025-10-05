using SmartAblationSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using static SmartAblationSystem.Models.AblationSummary;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class handles malicious data change model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    class MaliciousDataChangeModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private List<AblationReportChanges> ablationReportChanges = new List<Helpers.AblationReportChanges>();

        private static MaliciousDataChangeModel instance;
        private AblationSiteEnum ablationSite;
        private string notes;
        private string diagnosis;
        private string outcome;
        private bool isMaliciousDataChangeModelActivated = false;
        private bool isDataEdited = false;

        /// <summary>
        /// Returns a Notification Model object instance
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static MaliciousDataChangeModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MaliciousDataChangeModel();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets a Physician
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationSiteEnum AblationSite
        {
            get
            {
                return ablationSite;
            }
            set
            {
                ablationSite = value;
                NotifyPropertyChanged("AblationSite");
            }
        }
        /// <summary>
        /// Gets or sets the notes
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Notes
        {
            get
            {
               return notes;
            }
            set
            {
                notes = value;
                NotifyPropertyChanged("Notes");

            }
        }
        /// <summary>
        /// Gets or sets the diagnosis
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Diagnosis
        {
            get
            {
              return  diagnosis;
            }
            set
            {
                diagnosis = value;
                NotifyPropertyChanged("Diagnosis");
            }
        }
        /// <summary>
        /// Gets or sets the outcome
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Outcome
        {
            get
            {
                return outcome;
            }
            set
            {
                outcome = value;
                NotifyPropertyChanged("Outcome");
            }
        }

        /// <summary>
        /// Gets or sets an ablation report change list
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        internal List<AblationReportChanges> AblationReportChanges
        {
            get
            {
               return ablationReportChanges;
            }
            set
            {
                ablationReportChanges = value;
                NotifyPropertyChanged("AblationReportChanges");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether malicious data change model is activated
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary
        public bool IsMaliciousDataChangeModelActivated
        {
            get => isMaliciousDataChangeModelActivated;
            set => isMaliciousDataChangeModelActivated = value;
        }
        /// <summary>
        /// Gets or sets a value indicating whether is data edited
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary
        public bool IsDataEdited
        {
            get => isDataEdited;
            set => isDataEdited = value;
        }

        /// <summary>
        /// Initializes the instance of Notification Model
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void initialize()
        {
            instance = new MaliciousDataChangeModel();
        }

        /// <summary>
        /// Saves the notifications to the database
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="ablationReportChanges">ablation report changes</param>
        public void PopulateNewReoprtchange(AblationReportChanges ablationReportChanges)
        {
            AblationReportChanges _ablationReportChanges = AblationReportChanges?.FirstOrDefault(a => a.TreatmentNumber == ablationReportChanges.TreatmentNumber);

            if (_ablationReportChanges != null)
            {
                _ablationReportChanges.AblationSite = ablationReportChanges.AblationSite;
                _ablationReportChanges.Notes = ablationReportChanges.Notes;
                _ablationReportChanges.Diagnosis = ablationReportChanges.Diagnosis;
                _ablationReportChanges.Outcome = ablationReportChanges.Outcome;

            }
            else
            {
                AblationReportChanges.Add(ablationReportChanges);
            }
        }

        /// <summary>
        /// This class notifies listeners that a property changed
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="propertyName">The property name that has changed.</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
