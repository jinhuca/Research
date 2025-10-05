namespace Crystal.Exceptions
{
  internal class MakeGenericTypeFailedException : Exception
  {
    public MakeGenericTypeFailedException(ArgumentException innerException)
      : base("MakeGenericType failed", innerException)
    {
    }
  }
}
