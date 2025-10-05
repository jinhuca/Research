namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is the Tip or Balloon pressure selection Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    internal static class TipBalloonPressureSelection
    {
        private static bool tipPressureSelected = false;

        /// <summary>
        /// Gets or sets a value indicating whether the Tip pressure is selected or not
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool TipPressureSelected
        {
            get
            {
                return tipPressureSelected;
            }
            set
            {
                tipPressureSelected = value;
            }
        }
    }
}