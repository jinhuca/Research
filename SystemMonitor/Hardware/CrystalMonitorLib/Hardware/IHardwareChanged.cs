namespace CrystalMonitor.Hardware;

internal interface IHardwareChanged {
  event HardwareEventHandler HardwareAdded;
  event HardwareEventHandler HardwareRemoved;
}