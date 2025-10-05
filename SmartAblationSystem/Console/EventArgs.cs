using System;
using System.Collections.Generic;

namespace Console
{
    /// <summary>
    ///  Represents the pressure transducer event args class
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PressureTransducerEventArgs : System.EventArgs
    {
        private List<IPressureTransducer> pressureTransducer;

        /// <summary>
        ///  Pressure type enumeration
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum PressureType
        {
            TP = 0,
            CP = 1
        }

        /// <summary>
        /// Gets or sets the pressure type
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public PressureType Type
        { get; set; }

        /// <summary>
        /// Creates pressure transducer event class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="_PressureTransducer"></param>
        public PressureTransducerEventArgs(List<IPressureTransducer> _PressureTransducer)
        {
            this.PressureTransducer = _PressureTransducer;
        }

        /// <summary>
        /// Gets or sets the pressure transducer list
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<IPressureTransducer> PressureTransducer
        {
            get
            {
                return pressureTransducer;
            }

            set
            {
                pressureTransducer = value;
            }
        }
    }

    /// <summary>
    ///  Represents the thermocouple event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class ThermocoupleEventArgs : System.EventArgs
    {
        private List<IThermocouple> thermocouple;

        /// <summary>
        /// Thermocouple type enumeration
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum ThermocoupleType
        {
            TC = 0,
            TS = 1
        }

        /// <summary>
        /// Gets or sets the thermocouple type
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ThermocoupleType Type
        { get; set; }

        /// <summary>
        /// Creates thermocouple event  class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="_ThermocoupleEventArgs">list of thermocouple event args </param>
        public ThermocoupleEventArgs(List<IThermocouple> _ThermocoupleEventArgs)
        {
            this.Thermocouple = _ThermocoupleEventArgs;
        }

        /// <summary>
        /// Gets or sets the list of thermocouple
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<IThermocouple> Thermocouple
        {
            get
            {
                return thermocouple;
            }

            set
            {
                thermocouple = value;
            }
        }
    }

    //Pressure switch events

    /// <summary>
    ///  Represents the pressure switch event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class PressureSwitchEventArgs : System.EventArgs
    {
        private List<IPressureSwitch> pressureSwitch;

        /// <summary>
        /// Creates the  pressure switch event class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="_pressureSwitch">list of the pressure switch</param>
        public PressureSwitchEventArgs(List<IPressureSwitch> _pressureSwitch)
        {
            this.PressureSwitch = _pressureSwitch;
        }

        /// <summary>
        /// Gets or sets the list of the  pressureSwitch
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public List<IPressureSwitch> PressureSwitch
        {
            get
            {
                return pressureSwitch;
            }

            set
            {
                pressureSwitch = value;
            }
        }
    }

    // Flow meter event
    /// <summary>
    ///  Represents the flow meter event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class FlowMeterEventArgs : System.EventArgs
    {
        private IFlowMeter flowMeter;

        /// <summary>
        /// Creates the flow meter event class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="_flowMeter"> flow meter </param>
        public FlowMeterEventArgs(IFlowMeter _flowMeter)
        {
            this.FlowMeter = _flowMeter;
        }

        /// <summary>
        /// Gets or sets the flow meter
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public IFlowMeter FlowMeter
        {
            get
            {
                return flowMeter;
            }

            set
            {
                flowMeter = value;
            }
        }
    }

    // Load cell Event
    /// <summary>
    ///  Represents the load cell event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class LoadCellEventArgs : System.EventArgs
    {
        private ILoadCell loadCell;

        /// <summary>
        /// Creates load cell event class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="_loadCell">Load cell</param>
        public LoadCellEventArgs(ILoadCell _loadCell)
        {
            this.LoadCell = _loadCell;
        }

        /// <summary>
        /// Gets or sets the load cell
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ILoadCell LoadCell
        {
            get
            {
                return loadCell;
            }

            set
            {
                loadCell = value;
            }
        }
    }

    //Blood Detector

    /// <summary>
    ///  Represents the blood detector class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class BloodDetectorEventArgs : System.EventArgs
    {
        private IBloodDetector bloodDetector;

        /// <summary>
        /// Gets or sets the blood detector
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public IBloodDetector BloodDetector
        {
            get
            {
                return bloodDetector;
            }

            set
            {
                bloodDetector = value;
            }
        }

        /// <summary>
        /// Creates blood detector event class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="_iBloodDetector">Blood detector </param>
        public BloodDetectorEventArgs(IBloodDetector _iBloodDetector)
        {
            this.BloodDetector = _iBloodDetector;
        }
    }

    // Register Values Event

    /// <summary>
    ///  Represents the register values event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class RegisterValuesEventArgs : System.EventArgs
    {
        /// <summary>
        /// Register type enumeration
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public enum RegisterType
        {
            PatientMicrocontroller = 0,
            MainMicrocontroller = 1,  // Central Micro Controller
            ConnectionBox = 2,
        }

        /// <summary>
        /// Gets or sets the register type ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID
        { get; set; }

        /// <summary>
        /// Gets or sets the register type
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public RegisterType Type
        { get; set; }

        /// <summary>
        /// Creates register values event class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public RegisterValuesEventArgs()
        {
        }
    }

    /// <summary>
    ///  Represents the ablation event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AblationEventArgs : System.EventArgs
    {
        private int compter = 0;
        private double temperature;
        private int ablationID = 0;

        /// <summary>
        /// Creates ablation event  class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public AblationEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets the ablation compter
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int Compter
        {
            get
            {
                return compter;
            }

            set
            {
                compter = value;
            }
        }

        /// <summary>
        /// Gets or sets the temperature
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double Temperature
        {
            get
            {
                return temperature;
            }

            set
            {
                temperature = value;
            }
        }

        public int AblationID
        {
            get
            {
                return ablationID;
            }
            set
            {
                ablationID = value;
            }
        }
    }

    /// <summary>
    ///  Represents the inflation event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class InflationEventArgs : System.EventArgs
    {
        private System.TimeSpan timeSpent = System.TimeSpan.Zero;

        /// <summary>
        /// Creates inflation event  class
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public InflationEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets the time spent
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public TimeSpan TimeSpent
        {
            get
            {
                return timeSpent;
            }

            set
            {
                timeSpent = value;
            }
        }
    }

    /// <summary>
    ///  Represents the ecg event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class EcgEventArgs : System.EventArgs
    {
        private int compter = 0;
        private double ecgValue;

        // 0 for channel 1-2....3 for channel 9-10  Tip and Acceleromter

        private double[] ecgChannelData = new double[7];

        /// <summary>
        /// Creates class the Acceleromter...events
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public EcgEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets the compter
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int Compter
        {
            get
            {
                return compter;
            }

            set
            {
                compter = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ecg value
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double EcgValue
        {
            get
            {
                return ecgValue;
            }

            set
            {
                ecgValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ecg channel data
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double[] EcgChannelData
        {
            get
            {
                return ecgChannelData;
            }

            set
            {
                ecgChannelData = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ecg ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID { get; set; }
    }


    public class RemoteControlMembraneSwitchStateEventArgs : System.EventArgs
    {

        /// <summary>
        /// Creates  class the Acceleromter...events
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public RemoteControlMembraneSwitchStateEventArgs()
        {
        }


        /// <summary>
        /// Gets or sets the Ecg ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID { get; set; }
    }

    public class BloodPressureSensorEventArgs : System.EventArgs
    {

        /// <summary>
        /// Creates  class the BloodPressureSensor...events
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public BloodPressureSensorEventArgs()
        {
        }


        /// <summary>
        /// Gets or sets the BloodPressureSensor ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID { get; set; }
    }

    public class ProbeEventArgs : System.EventArgs
    {

        /// <summary>
        /// Creates  class the BloodPressureSensor...events
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ProbeEventArgs()
        {
        }


        /// <summary>
        /// Gets or sets the BloodPressureSensor ID
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public int ID { get; set; }
    }

    /// <summary>
    ///  Represents ablation timer event args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class AblationTimerEventArgs : System.EventArgs
    {
        private double seconds = 0;

        /// <summary>
        /// Creates ablation timer event class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="timer"></param>
        public AblationTimerEventArgs(double timer)
        {
            seconds = timer;
        }

        /// <summary>
        /// Gets or sets the Seconds
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public double Seconds
        {
            get
            {
                return this.seconds;
            }

            set
            {
                this.seconds = value;
            }
        }
    }

    /// <summary>
    ///  Represents blood pressure graph y-axis args class
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class OcclusionPressureGraphAxisYEventArgs : System.EventArgs
    {
        private string limitID = "";

        /// <summary>
        /// Creates occlusion pressure graph y-axis event class
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="limitID"></param>
        public OcclusionPressureGraphAxisYEventArgs(string id)
        {
            limitID = id;
        }

        /// <summary>
        /// Gets or sets the LimitID (Max or Min)
        ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public string LimitID
        {
            get
            {
                return this.limitID;
            }

            set
            {
                this.limitID = value;

            }
        }
    }
}