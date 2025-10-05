using System.Windows.Controls;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This is is the Main Window View Model's Interface
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal interface IMainWindowViewModel
    {
        /// <summary>
        /// Gets/sets the current view 
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        UserControl CurrentView { get; set; }

        /// <summary>
        ///  Change view
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="us"> user control</param>
        void changeView(UserControl us);
    }
}