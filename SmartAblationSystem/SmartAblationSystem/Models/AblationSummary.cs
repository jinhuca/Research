using Shared;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class is the Ablation Summary Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AblationSummary
    {
        /// <summary>
        /// Gets or sets the current ablation site value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationSiteEnum CurrentAblationSite
        {
            get;
            set;
        }

        private int totalRSPV = 0;
        private int totalRIPV = 0;
        private int totalLSPV = 0;
        private int totalLIPV = 0;
        private int totalOther = 0;
        private int totalLCPV = 0;
        private int totalRMPV = 0;
        private double totalRSPVDuration = 0;
        private double totalRIPVDuration = 0;
        private double totalLSPVDuration = 0;
        private double totalLIPVDuration = 0;
        private double totalOtherDuration = 0;
        private double totalLCPVDuration = 0;
        private double totalRMPVDuration = 0;

    /// <summary>
    /// This class is the Ablation Summary Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public AblationSummary()
        {
        }

        /// <summary>
        /// Gets or sets the Total RSVP value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TotalRSPV
        {
            get
            {
                return totalRSPV;
            }
            set
            {
                totalRSPV = value;
            }
        }

        /// <summary>
        /// Gets or sets the Total RIPV value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TotalRIPV
        {
            get
            {
                return totalRIPV;
            }
            set
            {
                totalRIPV = value;
            }
        }

        /// <summary>
        /// Gets or sets the total LSPV value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TotalLSPV
        {
            get
            {
                return totalLSPV;
            }
            set
            {
                totalLSPV = value;
            }
        }

        /// <summary>
        /// Gets or sets the Total LIPV value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TotalLIPV
        {
            get
            {
                return totalLIPV;
            }
            set
            {
                totalLIPV = value;
            }
        }

        /// <summary>
        /// Gets or sets the Total Other value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TotalOther
        {
            get
            {
                return totalOther;
            }
            set
            {
                totalOther = value;
            }
        }

        public int TotalLCPV
        {
          get => totalLCPV;
          set => totalLCPV = value;
        }

        public int TotalRMPV
        {
          get => totalRMPV;
          set => totalRMPV = value;
        }

    /// <summary>
    /// Gets or sets the total RSPV Duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TotalRSPVDuration
        {
            get
            {
                return totalRSPVDuration;
            }
            set
            {
                totalRSPVDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets the Total RIPV Duration value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TotalRIPVDuration
        {
            get
            {
                return totalRIPVDuration;
            }
            set
            {
                totalRIPVDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets the Total LSPV Duration value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TotalLSPVDuration
        {
            get
            {
                return totalLSPVDuration;
            }
            set
            {
                totalLSPVDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets the Total LIPV Duration value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TotalLIPVDuration
        {
            get
            {
                return totalLIPVDuration;
            }
            set
            {
                totalLIPVDuration = value;
            }
        }

        public double TotalLCPVDuration
        {
          get => totalLCPVDuration;
          set => totalLCPVDuration = value;
        }

        public double TotalRMPVDuration
        {
          get => totalRMPVDuration;
          set => totalRMPVDuration = value;
        }

    /// <summary>
    /// Gets or sets the Total Other Duration value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public double TotalOtherDuration
        {
            get
            {
                return totalOtherDuration;
            }
            set
            {
                totalOtherDuration = value;
            }
        }

        /// <summary>
        /// Returns the total Ablation number
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int TotalAblation
        {
            get
            {
              return TotalRSPV + TotalRIPV + TotalLSPV + TotalLIPV + TotalLCPV + totalRMPV + TotalOther;
            }
        }

        /// <summary>
        /// Returns the total ablation duration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double TotalAblationDuration
        {
            get
            {
              return TotalRSPVDuration + TotalRIPVDuration + TotalLSPVDuration + TotalLIPVDuration 
                     + TotalLCPVDuration + TotalRMPVDuration + TotalOtherDuration;
            }
        }

        /// <summary>
        /// Clears the Ablation Summary properties
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public void ClearAblationSummary()
        {
            if (ProcedureLogModel.CanReloadProcudreInformation)
            {
                TotalRSPV = 0;
                TotalRSPVDuration = 0;

                TotalRIPV = 0;
                TotalRIPVDuration = 0;

                TotalLSPV = 0;
                TotalLSPVDuration = 0;

                TotalLIPV = 0;
                TotalLIPVDuration = 0;

                TotalOther = 0;
                TotalOtherDuration = 0;

                TotalLCPV = 0;
                totalLCPVDuration = 0;

                TotalRMPV = 0;
                TotalRMPVDuration = 0;
            }
        }
    }
}