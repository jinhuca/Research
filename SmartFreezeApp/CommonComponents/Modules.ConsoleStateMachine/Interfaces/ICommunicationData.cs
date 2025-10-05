using System;
using System.Collections;

namespace Modules.ConsoleStateMachine.Interfaces;

public record MessagePayload(int Length, int Id, DateTime Timestamp)
{
  private const int MaxLength = 8;
  
  public int Id { get; set; }
  public DateTime Timestamp { get; set; }
  public BitArray Payload = new(MaxLength);
}