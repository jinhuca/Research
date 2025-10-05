using System;

namespace Crystal.Exceptions
{
  public class CircularDependencyException : Exception
    {
        public CircularDependencyException(Type type, string name)
            : base($"Circular reference: Type: {type}, Name: {name}")
        {
        }
    }
}
