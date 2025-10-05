using DataAccessLayer;
using FileSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SmartAblationSystem.Models
{
    /// <summary>
    /// This class handles procedure log
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public static class ProcedureLogModel
    {

        readonly static int SkinToSkinTemperature = Properties.Settings.Default.SkinToSkinTemperature;

        public static Tuple<bool, bool, bool> AblationTimersSet = Tuple.Create(true, false, false);

        private static DateTime lastTreatmnetDate = new DateTime(2010, 9, 8); 
        private static readonly DateTime referenceDate = new DateTime(2010, 9, 8);

        private static bool isUserAccessRecord = false;

        /// <summary>
        /// Gets or sets previous patient info
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Patient PreviousLogedPatient
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a previous procedure
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Procedure PreviousProcedure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a previous ablation
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static Ablation PreviousAblation
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a previous ablation summary
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static AblationSummary PreviousAblationSummary
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets can reload procedure info
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool CanReloadProcudreInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a previous all ablation data list
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static List<List<AblationDataDetails>> PreviousAllAblationDataList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets skin to skin duration value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static int SkinToSkinDuration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets skin to skin duration before leaving the cryo screen value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static int SkinToSkinDurationBeforeLeavingTheCryoScreen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a decreasing compter value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static int DecreasingCompter
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets previous TC1 reading
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static double PreviousTC1Reading
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets reference date
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static  DateTime ReferenceDate
        {
            get => referenceDate;
        }

        /// <summary>
        /// Gets or sets last treatment date
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static DateTime LastTreatmnetDate
        {
            get => lastTreatmnetDate;
            set => lastTreatmnetDate = value;
        }

        /// <summary>
        /// Gets or sets is user access record value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static bool IsUserAccessRecord
        {
            get => isUserAccessRecord;
            set => isUserAccessRecord = value;
        }

        /// <summary>
        /// Tracks skin to skin duration
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void TrackSkinToSkinDuration(double catheterTemperature, Communication.CanBusMessageDefinition.MessageStateId systemState, bool isCatheterCableConnected,   bool IsStillCatherInTheBody = false)
        {

            if ((catheterTemperature >= SkinToSkinTemperature || SkinToSkinDuration != 0) && isCatheterCableConnected)
            {
                if (systemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY &&
                    systemState != Communication.CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE)
                {
                    DecreasingCompter = 0;
                    SkinToSkinDuration++;
                }
                else
                {
                    if (catheterTemperature >= SkinToSkinTemperature)
                    {
                        DecreasingCompter = 0;
                        SkinToSkinDuration++;
                    }
                    else
                    {
                        if (SkinToSkinDuration != 0)
                        {

                            if (DecreasingCompter == 0)
                            {
                                PreviousTC1Reading = catheterTemperature;
                            }
                            else if (DecreasingCompter > 30)
                            {
                                double DeltaTemperature = PreviousTC1Reading - catheterTemperature;
                                if (DeltaTemperature > 3)
                                    return;
                            }
                            DecreasingCompter++;
                            SkinToSkinDuration++;
                        }

                    }

                }
            }
        }

        /// <summary>
        /// Resets information
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public static void ResetInformation()
        {
            PreviousProcedure = null;
            PreviousAblationSummary = null;
            CanReloadProcudreInformation = false;
            PreviousAllAblationDataList = null;
            SkinToSkinDuration = 0;
            SkinToSkinDurationBeforeLeavingTheCryoScreen = 0;
            DecreasingCompter = 0;
            PreviousTC1Reading = 0;
        }
    }
}
