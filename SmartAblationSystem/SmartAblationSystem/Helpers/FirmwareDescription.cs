using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartAblationSystem.Helpers.Enumeration;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class gets/sets firmware description
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class FirmwareDescription : INotifyPropertyChanged
    {
        int id;
        string name;
        bool update = false;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///Gets/sets firmware description
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public FirmwareDescription(int id , String name, bool update = false)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        ///Gets/sets Id
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int Id
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        ///Gets/sets Name
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        ///Gets/sets update value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool Update
        {
            get => update;
            set
            {
                update = value;
                NotifyPropertyChanged("Update");
            }
        }
        /// <summary>
        ///Handle notify property change event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        internal void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
