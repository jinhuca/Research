using SmartAblationSystem.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class DocumentViewerWindow : Window
    {
        const string DocumentsFolderName = "Documentation";

        /// <summary>
        /// Initializes UserManualWindow components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public DocumentViewerWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Initializes DocumentViewerWindow components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// Fix Bug #552: In the User Manual window, searching result popup problem. 
        public DocumentViewerWindow(string documentName, string windowTitle)
        {
            InitializeComponent();

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                       DocumentsFolderName,  documentName);

            TitleLabel.Content = windowTitle;
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    this.documentViewer.Document = new XpsDocument(path, FileAccess.Read).GetFixedDocumentSequence();
                   // ScrollViewer fs = this.documentViewer.Template.FindName("PART_ContentHost", this.documentViewer) as ScrollViewer;
                   
                    ContentControl cc = this.documentViewer.Template.FindName("PART_FindToolBarHost", this.documentViewer) as ContentControl;
                    cc.Visibility = Visibility.Collapsed;

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }



    /// <summary>
    /// Occurs when the Yes_Click event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Contains state information and event data associated with a routed event.</param>
    private void Yes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Task.Delay(2000).ContinueWith(t => CommonViewModel.Current.IsUserManualOpned = false);
                this.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

    }
}