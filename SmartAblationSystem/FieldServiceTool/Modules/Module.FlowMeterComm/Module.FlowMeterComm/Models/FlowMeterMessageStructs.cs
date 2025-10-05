using System.Runtime.InteropServices;

namespace Module.FlowMeterComm.Models
{

  public enum FlowMeterCommand
  {
    ReadUniqueIdentifier = 0,
    ReadFlowRate = 1,
    ReadAllVariables = 3,
    QueryDeviceId = 11
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct DeviceIdMessage
  {
    public byte MessageSize;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Status;

    public byte Reserved1; // 0xFE
    public byte ManufacturerCode;
    public byte DeviceTypeCode;
    public byte NumOfPreambles;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] RevisionNumbers;

    public byte Flags;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] DeviceId;

    public byte CheckSum;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct FlowRateMessage
  {
    public byte MessageSize;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Status;

    public byte Unit;
    
    [MarshalAs(UnmanagedType.R4)] 
    public float FlowRate;

    public byte CheckSum;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct AllVariablesMessage
  {
    public byte MessageSize;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Status;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] CurrentVoltage; 

    public byte FlowUnit;

    [MarshalAs(UnmanagedType.R4)]
    public float FlowRate;

    public byte TemperatureUnit;

    [MarshalAs(UnmanagedType.R4)]
    public float Temperature;

    public byte CheckSum;
  }

}