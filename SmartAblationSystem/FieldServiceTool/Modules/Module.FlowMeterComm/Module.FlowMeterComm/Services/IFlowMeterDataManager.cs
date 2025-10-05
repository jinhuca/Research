using System;

namespace Module.FlowMeterComm.Services
{
  public interface IFlowMeterDataManager
  {
    string[] AvailableComPorts { get; }
    IObservable<string> CommunicationMessageObserver { get; }
    bool IsConnectionLost { get; }

    bool ConnectToFlowMeter(string portName = null);
    void CloseConnection();
    void StartCollectingData(int samplingTime = 0);
    void StopCollectingData();
    FlowMeterValidationResult ValidateFlowMeter();
  }
}