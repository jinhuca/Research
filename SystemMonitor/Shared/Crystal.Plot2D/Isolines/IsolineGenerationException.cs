using System;
using System.Runtime.Serialization;

namespace Crystal.Plot2D.Isolines;

/// <summary>
///   Exception that is thrown when error occurs while building isolines.
/// </summary>
public sealed class IsolineGenerationException : Exception {
  internal IsolineGenerationException() { }
  internal IsolineGenerationException(string message) : base(message) { }
  internal IsolineGenerationException(string message, Exception inner) : base(message, inner) { }
}
      