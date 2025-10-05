using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    public class DiaphragmConditioning : INotifyPropertyChanged
    {
        double amplitudeReference = 0;

        bool isDiaphragmReseting = false;

        public DiaphragmConditioning(double _amplitudeReference)
        {
            AmplitudeReference = _amplitudeReference;
        }

        public double AmplitudeReference
        {
            get
            {
                return amplitudeReference;
            }

            set
            {
                amplitudeReference = value;
                NotifyPropertyChanged("AmplitudeReference");


            }
        }

        public bool IsDiaphragmReseting
        {
            get => isDiaphragmReseting;
            set => isDiaphragmReseting = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This class notifies listeners that a property changed
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="propertyName">The property name that has changed.</param>
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
