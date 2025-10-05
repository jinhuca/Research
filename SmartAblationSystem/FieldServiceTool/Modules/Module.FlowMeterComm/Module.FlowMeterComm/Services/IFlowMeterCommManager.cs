using System;
using Module.FlowMeterComm.Models;

namespace FlowMeterComm
{
  public interface IFlowMeterCommManager
  {
    event EventHandler<SerialComErrorEventArgs> FlowMeterCommErrorEvent;
    IFlowMeterParameters FlowMeterParameters { get; }
    IObservable<string> CommMessageObservable { get; }
    bool InitCommunication(string comPortName);
    string[] GetComNames();
    bool ConnectToDevice(string serialNum = "");
    void StartReading(int interval);
    void StopReading();
    void Close();
    void RequestToQueryDeviceIdBySerialNum(string serialNum);
    void RequestToQueryDeviceId();
    void RequestToReadAllVariables();
    void RequestToReadFlowRate();
    float ReadFlowRateSync();
  }
}