namespace Modules.Infrastructure.Definitions;

public enum ConsoleStateId
{
  UnknownStateId = 0,
  IdleStateId = 256,
  ReadyStateId = 512,
  InflationStateId = 768,
  TransitionStateId = 1024,
  AblationStateId = 1280,
  ThawingStateId = 1536,
  ExceptionStateId = 1792
}

