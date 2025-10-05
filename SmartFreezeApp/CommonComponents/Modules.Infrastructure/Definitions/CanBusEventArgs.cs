namespace Modules.Infrastructure.Definitions;

public class CanBusEventArgs : EventArgs
{
  public int Flags { get; set; }
  public int Cob { get; set; }
  public uint Id { get; set; }
  public short Length { get; set; }
  public byte[] Data { get; set; }
}