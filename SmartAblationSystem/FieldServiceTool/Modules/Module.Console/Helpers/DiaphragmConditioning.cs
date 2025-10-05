using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Module.Console.Helpers
{
  public class DiaphragmConditioning : BindableBase
  {
    double amplitudeReference = 0;
    static bool isDiaphragmReseting = false;

    public DiaphragmConditioning(double _amplitudeReference)
    {
      AmplitudeReference = _amplitudeReference;
    }

    public double AmplitudeReference
    {
      get => amplitudeReference;
      set => SetProperty(ref amplitudeReference, value);
    }

    public static bool IsDiaphragmReseting
    {
      get => isDiaphragmReseting;
      set => isDiaphragmReseting = value;
    }
  }
}
