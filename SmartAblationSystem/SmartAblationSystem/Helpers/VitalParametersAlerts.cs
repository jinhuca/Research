using Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handle vital parameters alerts
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class VitalParametersAlerts
    {

        private double MaxDiaphragmMovementValue => 100;

        /// <summary>
        /// Constructor that initialize vital parameters alerts.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public VitalParametersAlerts()
        {
        }

        /// <summary>
        /// Function that is driving diaphragm movement alert.
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0034</id>
        /// <param name="isDiaphragmMovementDetected">is diaphragm movement detected</param>
        /// <param name="isDiaphragmAmplitudeThresholdReached">is diaphragm amplitude threshold reached</param>
        /// <param name="systemState"></param>
        /// <param name="lastDiaphragmMovementPercentageOrGReadingValue">last diaphragm movement percentage or reading value</param>
        /// <returns>Return boolean value of should diaphragm movement alert trigged</returns>
        public bool ShouldDiaphragmMovementAlertTrigged(bool isDiaphragmMovementDetected, bool isDiaphragmAmplitudeThresholdReached, CanBusMessageDefinition.MessageStateId systemState, double lastDiaphragmMovementPercentageOrGReadingValue)
        {
            if ((isDiaphragmAmplitudeThresholdReached) && (lastDiaphragmMovementPercentageOrGReadingValue < MaxDiaphragmMovementValue) &&
                (systemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_TRANSITION || systemState == CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_ABLATION))
                return true;
            return false;
        }


        /// <summary>
        /// Function that is driving esophagus temperature alert.
        /// . Safety classification: Non-SERIOUS INJURY is possible (IEC 62304 Class B).
        /// </summary>
        /// <id>SF-SDS-0035</id>
        /// <param name="isEsophagusTemperatureThresholdReached">is esophagus temperature threshold reached</param>
        /// <param name="systemState">system state</param>
        /// <returns> Return boolean value of should esophagus temperature alert trigged </returns>
        public bool ShouldEsophagusTemperatureAlertTrigged(bool isEsophagusTemperatureThresholdReached, CanBusMessageDefinition.MessageStateId systemState)
        {
            if (isEsophagusTemperatureThresholdReached && (systemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_IDLE &&
                                                         systemState != CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_READY))
                return true;
            return false;
        }
    }
}
