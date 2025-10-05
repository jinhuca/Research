using Console;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module.Console.Helpers;
using Prism.Mvvm;

namespace Module.Console.Helpers
{
  /// <summary>
  /// This class is the change balloon type Finite-state machine.
  /// Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
  /// </summary>
  public class ChangeBalloonTypeFSM : BindableBase
  {
    private Enumeration.CatheterType catheterType = Enumeration.CatheterType.ID_UNKNOWN_mm;
    private bool dASBalloonEnabled = false;
    private InflateDeflateBalloonModel inflateDeflateBalloonModel;

    /// <summary>
    /// Creates the inflate deflate balloon model class
    /// . Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
    /// </summary>
    /// <id>SF-SDS-0027</id>
    /// <param name="inflateDeflateBalloonModel"> inflateDeflateBalloonModel object</param>
    /// <param name="data">Data</param>
    /// <param name="console">Console</param>
    public ChangeBalloonTypeFSM(InflateDeflateBalloonModel inflateDeflateBalloonModel, Data data, Machine console)
    {
      InflateDeflateBalloonModel = new InflateDeflateBalloonModel(data, console);
    }

    /// <summary>
    /// Gets or sets the catheter Type
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A). 
    /// </summary>
    public Enumeration.CatheterType CatheterType
    {
      get => catheterType;
      set => SetProperty(ref catheterType, value);
    }

    /// <summary>
    /// Gets or sets whether POLARx-TM-FIT is Enabled 
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public bool DASBalloonEnabled
    {
      get => dASBalloonEnabled;
      set
      {
        SetPressureSetpoint(value);
        dASBalloonEnabled = value;
        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// Get or sets the inflate deflate balloon model
    ///. Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public InflateDeflateBalloonModel InflateDeflateBalloonModel
    {
      get => inflateDeflateBalloonModel;
      set
      {
        inflateDeflateBalloonModel = value;
        RaisePropertyChanged();
      }
    }

    /// <summary>
    /// Sets the balloon pressure set point
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="dASBalloonEnabled"></param>
    private void SetPressureSetpoint(bool dASBalloonEnabled)
    {
      if (dASBalloonEnabled)
      {
        InflateDeflateBalloonModel.CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine = InflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsActivated;
        InflateDeflateBalloonModel.CurrentPressureSetpoint = InflateDeflateBalloonModel.HighPressureSetPoint;
      }
      else
      {
        InflateDeflateBalloonModel.CurrentFlowAndPressureRegulatorValueAccordingToTheStateMachine = InflateDeflateBalloonModel.CentralMicroControllerFlowAndPressureRegulatorValueAccordingToTheStateMachineWhenDASIsNotActivated;
        InflateDeflateBalloonModel.CurrentPressureSetpoint = InflateDeflateBalloonModel.LowPressureSetpoint;
      }
    }
  }
}
