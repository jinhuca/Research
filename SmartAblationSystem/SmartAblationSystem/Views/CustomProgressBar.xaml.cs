using System.Windows;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for CustomProgressBar.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class CustomProgressBar : Window
    {
        private static CustomProgressBar instance;

        /// <summary>
        /// Returns the instance of Custom Progress Bar.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static CustomProgressBar Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CustomProgressBar();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes the CustomProgressBar components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public CustomProgressBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows a dialog.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        public void Show(object sender)
        {
            this.ShowDialog();
        }
    }
}