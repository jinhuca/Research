using Communication;
using Console;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using static Communication.CanBusMessageDefinition;

namespace SmartAblationSystem.Models
{
  /// <summary>
  /// This class is the Inflate deflate balloon model
  /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
  /// </summary>
  public class InflateDeflateBalloonModel
  {
    private double lowPressureSetpoint = 2.5;
    private double highPressureSetPoint = 8;
    private double lowFlowSetPoint = 7800;
    private double highFlowSetPoint = 8500;
    private double rampUpTimeByStep = 0;
    private double pressureRampUpValue = 0;
    private double rampDownTimeByStep = 0;
    private double pressureRampDownValue = 0;
    private double totalRampUpTime = 0;
    private double totalRampDowntime = 0;
    private double currentPressureSetpoint = 2.5;
    private CanBusMessageDefinition.MessageStateId state = CanBusMessageDefinition.MessageStateId.CAN_ID_STATE_UNKNOWN;


    /// <summary>
    /// Creates the inflate deflate balloon model class
    /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
    /// </summary>
    /// <id>SF-SDS-0026</id>
    /// <param name="data">Data</param>
    /// <param name="console">Console</param>
    public InflateDeflateBalloonModel(Data data, Machine console)
    {
      CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated = new Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 0} },
                {MessageStateId.CAN_ID_STATE_READY, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 150} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 200} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 600} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 7800, TargetInjectionPressure = 0} },
                {MessageStateId.CAN_ID_STATE_THAWING, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 200} },
            };

      CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated = new Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 0} },
                {MessageStateId.CAN_ID_STATE_READY, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 100} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 150} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 560} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 8500, TargetInjectionPressure = 0} },
                {MessageStateId.CAN_ID_STATE_THAWING, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 150} },
            };



      CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine = new Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator>()
            {
                {MessageStateId.CAN_ID_STATE_IDLE,   new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 0} },
                {MessageStateId.CAN_ID_STATE_READY, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 100} },
                {MessageStateId.CAN_ID_STATE_INFLATION,  new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 150} },
                {MessageStateId.CAN_ID_STATE_TRANSITION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 560} },
                {MessageStateId.CAN_ID_STATE_ABLATION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 8500, TargetInjectionPressure = 0} },
                {MessageStateId.CAN_ID_STATE_THAWING, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 150} },
            };

      InitializeDASBalloonRegisters(data, console);
    }

    /// <summary>
    /// Creates the inflate deflate balloon model class
    /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
    /// </summary>
    /// <param name="lowPressureSetpoint">Low pressure set point value</param>
    /// <param name="highPressureSetPoint">High pressure set point value</param>
    /// <param name="lowFlowSetPoint">Low flow set point value</param>
    /// <param name="highFlowSetPoint">High flow set point value</param>
    /// <param name="rampUpTimeByStep">Ramp up time by step value</param>
    /// <param name="pressureRampUpValue">Pressure ramp up value</param>
    /// <param name="rampDownTimeByStep">Ramp down time by step value</param>
    /// <param name="pressureRampDownValue">Pressure ramp down value</param>
    /// <param name="totalRampUpTime">Total ramp up time value</param>
    /// <param name="totalRampDowntime">Total ramp down time value</param>
    /// <param name="state">System state</param>
    /*public InflateDeflateBalloonModel(double lowPressureSetpoint, double highPressureSetPoint, double lowFlowSetPoint, double highFlowSetPoint, double rampUpTimeByStep,
        double pressureRampUpValue, double rampDownTimeByStep, double pressureRampDownValue, double totalRampUpTime, double totalRampDowntime, CanBusMessageDefinition.MessageStateId state)
    {


        LowPressureSetpoint = lowPressureSetpoint;
        HighPressureSetPoint = highPressureSetPoint;
        LowFlowSetPoint = lowFlowSetPoint;
        HighFlowSetPoint = highFlowSetPoint;
        RampUpTimeByStep = rampUpTimeByStep;
        PressureRampUpValue = pressureRampUpValue;
        RampDownTimeByStep = rampDownTimeByStep;
        PressureRampDownValue = pressureRampDownValue;
        TotalRampUpTime = totalRampUpTime;
        TotalRampDowntime = totalRampDowntime;
        State = state;

        CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated = new Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator>()
        {
            {MessageStateId.CAN_ID_STATE_IDLE,   new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 8} },
            {MessageStateId.CAN_ID_STATE_READY, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 8} },
            {MessageStateId.CAN_ID_STATE_INFLATION,  new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 8} },
            {MessageStateId.CAN_ID_STATE_TRANSITION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 8} },
            {MessageStateId.CAN_ID_STATE_ABLATION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 7800, TargetInjectionPressure = 8} },
            {MessageStateId.CAN_ID_STATE_THAWING, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 8} },
        };

        CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated = new Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator>()
        {
            {MessageStateId.CAN_ID_STATE_IDLE,   new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 2.5} },
            {MessageStateId.CAN_ID_STATE_READY, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 2.5} },
            {MessageStateId.CAN_ID_STATE_INFLATION,  new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 2.5} },
            {MessageStateId.CAN_ID_STATE_TRANSITION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 2.5} },
            {MessageStateId.CAN_ID_STATE_ABLATION, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 8500, TargetInjectionPressure = 2.5} },
            {MessageStateId.CAN_ID_STATE_THAWING, new CentralMicroControllerFlowAndPressureRegulator {  TargetInjectionFlow = 0, TargetInjectionPressure = 2.5} },
        };

    }*/

    /// <summary>
    /// Gets or sets low pressure set point
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double LowPressureSetpoint { get => lowPressureSetpoint; set => lowPressureSetpoint = value; }

    /// <summary>
    /// Gets or sets high pressure set point
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double HighPressureSetPoint { get => highPressureSetPoint; set => highPressureSetPoint = value; }

    /// <summary>
    /// Gets or sets high flow set point
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double HighFlowSetPoint { get => highFlowSetPoint; set => highFlowSetPoint = value; }

    /// <summary>
    /// Gets or sets low flow set point
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double LowFlowSetPoint { get => lowFlowSetPoint; set => lowFlowSetPoint = value; }

    /// <summary>
    /// Gets or sets ramp up time by step
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double RampUpTimeByStep { get => rampUpTimeByStep; set => rampUpTimeByStep = value; }

    /// <summary>
    /// Gets or sets pressure ramp up value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double PressureRampUpValue { get => pressureRampUpValue; set => pressureRampUpValue = value; }

    /// <summary>
    /// Gets or sets ramp down time by step
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double RampDownTimeByStep { get => rampDownTimeByStep; set => rampDownTimeByStep = value; }

    /// <summary>
    /// Gets or sets pressure ramp down value
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double PressureRampDownValue { get => pressureRampDownValue; set => pressureRampDownValue = value; }

    /// <summary>
    /// Gets or sets total ramp up time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double TotalRampUpTime { get => totalRampUpTime; set => totalRampUpTime = value; }

    /// <summary>
    /// Gets or sets total ramp down time
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double TotalRampDowntime { get => totalRampDowntime; set => totalRampDowntime = value; }

    /// <summary>
    /// Gets or sets the system state
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public CanBusMessageDefinition.MessageStateId State { get => state; set => state = value; }

    /// <summary>
    /// Gets or sets balloon current pressure set point
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public double CurrentPressureSetpoint
    {
      get => currentPressureSetpoint;
      set => currentPressureSetpoint = value;
    }

    /// <summary>
    /// Gets or sets the central microcontroller flow and pressure regulator value according to the state machine when POLARx-TM-FIT is activated
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator> CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the central microcontroller flow and pressure regulator value according to the state machine when POLARx-TM-FIT is not activated
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator> CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets current flow and pressure regulator value according to the state machine
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public Dictionary<MessageStateId, CentralMicroControllerFlowAndPressureRegulator> CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine
    {
      get;
      set;
    }

    /// <summary>
    /// Initializes POLARx-TM-FIT registers
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    /// <param name="data">Data</param>
    /// <param name="console">Console</param>
    private void InitializeDASBalloonRegisters(Data data, Machine console)
    {
      MessageStateId mid = MessageStateId.CAN_ID_STATE_IDLE;

      List<BalloonParameters> ballonParameters = data.DataAccess.GetDASBallonParameters();

      foreach (MessageStateId stateId in Enum.GetValues(typeof(MessageStateId)))
      {

        if (stateId != MessageStateId.CAN_ID_STATE_UNKNOWN && stateId != MessageStateId.CAN_ID_STATE_EXCEPTION)
        {
          int state = 0;
          state = Array.IndexOf(Enum.GetValues(typeof(MessageStateId)), stateId);

          switch (state)
          {
            case 1:
              mid = MessageStateId.CAN_ID_STATE_IDLE;
              break;

            case 2:
              mid = MessageStateId.CAN_ID_STATE_READY;
              break;

            case 3:
              mid = MessageStateId.CAN_ID_STATE_INFLATION;
              break;

            case 4:
              mid = MessageStateId.CAN_ID_STATE_TRANSITION;
              break;

            case 5:
              mid = MessageStateId.CAN_ID_STATE_ABLATION;
              break;

            case 6:
              mid = MessageStateId.CAN_ID_STATE_THAWING;
              break;

          }

          BalloonParameters _ballonParameters = ballonParameters[state - 1];

          LowPressureSetpoint = (double)_ballonParameters.LowPressureSetpoint;
          HighPressureSetPoint = (double)_ballonParameters.HighPressureSetPoint;

          CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionFlow = (double)_ballonParameters.HighFlowSetPoint;
          CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated[mid].TargetInjectionPressure = (double)_ballonParameters.HighTargetInjectionPressure;

          CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionFlow = (double)_ballonParameters.LowFlowSetPoint;
          CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated[mid].TargetInjectionPressure = (double)_ballonParameters.LowTargetInjectionPressure;

          //Balloon Rum up and ramp dow timing 
          console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampUpTimeByStep = (double)_ballonParameters.RampUpTimeByStep;
          console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampUpValue = (double)_ballonParameters.PressureRampUpValue;
          console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].RampDownTimeByStep = (double)_ballonParameters.RampDownTimeByStep;
          console.PatientMicroControllerCryoBalloonConfigurationValueAccordingToTheStateMachine[mid].PressureRampDownValue = (double)_ballonParameters.PressureRampDownValue;
        }
      }

      CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine = CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated;
      CurrentPressureSetpoint = LowPressureSetpoint;
    }
  }
}
