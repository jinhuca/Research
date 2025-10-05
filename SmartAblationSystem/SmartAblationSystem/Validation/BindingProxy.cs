using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartAblationSystem.Validation
{
    /// <summary>
    /// This class makes sure that the validation rules can access the DataContex
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class BindingProxy : System.Windows.Freezable
    {
        //This class is needed so the Validation rules can access the DataContext
        //https://social.technet.microsoft.com/wiki/contents/articles/31422.wpf-passing-a-data-bound-value-to-a-validation-rule.aspx

        /// <summary>
        /// Returns Binding proxy
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
        /// <summary>
        /// Gets or sets Data property vlaue.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}
