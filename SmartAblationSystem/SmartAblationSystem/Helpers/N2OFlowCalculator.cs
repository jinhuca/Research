using System;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Helpers
{
    /// <summary>
    /// This class handles N2O flow calculation
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class N2OFlowCalculator
    {
        private  MessageStateId currentState = MessageStateId.CAN_ID_STATE_UNKNOWN;
        private MessageStateId  previousState = MessageStateId.CAN_ID_STATE_UNKNOWN;

        private double aCoefficient = 0;
        private double bCoefficient = 0;
        private double cCoefficient = 0;

        private int virtualTime = 0;

        /// <summary>
        /// Constructor that initialize N2O flow calculator
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public N2OFlowCalculator()
        {
        }

        /// <summary>
        /// Gets/sets current state value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MessageStateId CurrentState { get => currentState; set => currentState = value; }

        /// <summary>
        /// Gets/sets previous state value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public MessageStateId PreviousState { get => previousState; set => previousState = value; }

        // Used for FM1FlowMeterLowRangeLimit = fm1LowFit;

        /// <summary>
        /// Gets/sets aCoefficient value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ACoefficient { get => aCoefficient; set => aCoefficient = value; }

        // Used for  FM1FlowMeterHighRangelimit = fm1LowCeiling;

        /// <summary>
        /// Gets/sets bCoefficient value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double BCoefficient { get => bCoefficient; set => bCoefficient = value; }

        // Used for FM1FlowMeterThresholLowlimit = fm1LowOffset;
       
        /// <summary>
        /// Gets/sets cCoefficient value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double CCoefficient { get => cCoefficient; set => cCoefficient = value; }

        /// <summary>
        /// Gets/sets virtual time
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int VirtualTime { get => virtualTime; set => virtualTime = value; }

        /// <summary>
        /// Gets/sets expected flow value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double ExpectedFlow( MessageStateId state, double FM1FlowMeterLowRangeLimit, double FM1FlowMeterHighRangelimit, double FM1FlowMeterThresholLowlimit, int elapsedTimeInMilliseconds = 100)
        {
            double expectedFlow = 0;

            if (state != PreviousState)
            {
                VirtualTime = 0;
                SetQuadraticPolynomialcoefficients(FM1FlowMeterLowRangeLimit, FM1FlowMeterHighRangelimit, FM1FlowMeterThresholLowlimit);
            }
            else
            {
                VirtualTime++;
            }


            if (ACoefficient == 0)
            {
                expectedFlow = BCoefficient * VirtualTime + CCoefficient;
            }
            else
            {
                expectedFlow = ACoefficient * Math.Pow(VirtualTime, 2) + CCoefficient;

            }

            if (BCoefficient == 0)
            {

                expectedFlow = CCoefficient;
            }

            else if (expectedFlow > FM1FlowMeterHighRangelimit)
            {
                expectedFlow = FM1FlowMeterHighRangelimit;
            }


            PreviousState = state;

            return ( expectedFlow < 0 ? 0 : expectedFlow) ;
        }

        /// <summary>
        /// Sets quadratic polynomial coefficient value
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private void SetQuadraticPolynomialcoefficients(double FM1FlowMeterLowRangeLimit, double FM1FlowMeterHighRangelimit, double FM1FlowMeterThresholLowlimit)
        {
            double FM1LOWPOINTS = 250; 

            ACoefficient = FM1FlowMeterLowRangeLimit/100;

            if (FM1LOWPOINTS != 0)
            {
                BCoefficient = FM1FlowMeterHighRangelimit / FM1LOWPOINTS;
            }

            else
            {
                //TODO
            }
            CCoefficient = FM1FlowMeterThresholLowlimit;

        }
         
    }
}
