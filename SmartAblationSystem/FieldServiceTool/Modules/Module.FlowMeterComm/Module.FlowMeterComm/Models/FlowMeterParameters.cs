using FlowMeterComm;
using Prism.Mvvm;

namespace Module.FlowMeterComm.Models
{
  public class FlowMeterParameters : BindableBase, IFlowMeterParameters
  {
    private float _flowRate;

    public float FlowRate
    {
      get => _flowRate;
      set => SetProperty(ref _flowRate, value);
    }
  }
}