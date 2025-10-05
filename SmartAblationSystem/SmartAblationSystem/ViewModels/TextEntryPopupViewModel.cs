using Prism.Mvvm;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the Text Entry Popup View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal class TextEntryPopupViewModel : BindableBase
    {
        private CommonViewModel.TextEntryType entryType;

        /// <summary>
        /// This constructor initialize the class properties (not required here)
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public TextEntryPopupViewModel()
        {
        }

        /// <summary>
        /// Property that gets/sets the entry type
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public CommonViewModel.TextEntryType EntryType
        {
            get
            {
                return entryType;
            }
            set
            {
                entryType = value;
                RaisePropertyChanged("EntryType");
            }
        }
    }
}