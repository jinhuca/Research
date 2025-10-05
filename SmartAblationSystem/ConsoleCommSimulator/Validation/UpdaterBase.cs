using ConsoleCommSimulator.Interfaces;
using System;

namespace ConsoleCommSimulator.Validation
{
  public class UpdaterBase: IUpdater
  {
    public virtual void PublishUpdate(EventArgs args)
    {
    }

  }
}
