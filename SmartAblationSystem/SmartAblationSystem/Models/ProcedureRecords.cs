using DataAccessLayer;
using System;
using System.ComponentModel;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is the procedure records model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ProcedureRecords : INotifyPropertyChanged
    {
        private Procedure procedure;
        private bool selected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This class is the Procedure Records Model
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ProcedureRecords()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Procedure Records Model class and its properties
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="procedure">A Procedure representing a Procedure Record.</param>
        public ProcedureRecords(Procedure procedure)
        {
            this.Procedure = procedure;
        }

        /// <summary>
        /// Gets or sets a Patient
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Patient Patient
        {
            get { return Procedure?.Patient; }
            set { Procedure.Patient = value; }
        }

        /// <summary>
        /// Gets or sets a Procedure
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public Procedure Procedure
        {
            get { return procedure; }
            set { procedure = value; }
        }

        /// <summary>
        /// Gets the Patient's full name
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PatientFullName
        {
            get { return Patient?.FullName; }
        }

        /// <summary>
        /// Gets or sets the Patient's first name
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PatientFirstName
        {
            get { return Patient?.FirstName; }
        }

        /// <summary>
        /// Gets the Patient's last name
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string PatientLastName
        {
            get { return Patient?.LastName; }
        }

        /// <summary>
        /// Gets the Procedure's date
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DateTime ProcedureDate
        {
            get { return (DateTime)Procedure?.ProcedureStartDateTime; }
        }

        /// <summary>
        /// Gets whether the procedure is selected
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                NotifyPropertyChanged("Selected");
            }
        }
        /// <summary>
        /// Handles notify property changed event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}