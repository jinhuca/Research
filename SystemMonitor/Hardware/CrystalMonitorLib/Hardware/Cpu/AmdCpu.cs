namespace CrystalMonitor.Hardware.Cpu;

internal abstract class AmdCpu(int processorIndex, CpuId[][] cpuId, ISettings settings)
    : GenericCpu(processorIndex, cpuId, settings);
