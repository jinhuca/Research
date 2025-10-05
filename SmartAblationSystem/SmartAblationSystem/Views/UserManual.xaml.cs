using System.Windows;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for UserManual.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class UserManual : UserControl
    {
        /// <summary>
        /// Initializes UserManual components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserManual()
        {
            InitializeComponent();

            DocumentViewer dv1 = LogicalTreeHelper.FindLogicalNode(this, "documentViewer") as DocumentViewer;

        }
    }
}