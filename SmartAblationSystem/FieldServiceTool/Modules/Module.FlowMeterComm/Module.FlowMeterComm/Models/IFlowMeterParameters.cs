using System.ComponentModel;

namespace Module.FlowMeterComm.Models
{
  public interface IFlowMeterParameters : INotifyPropertyChanged
  {
    float FlowRate { get; set; }
  }
}