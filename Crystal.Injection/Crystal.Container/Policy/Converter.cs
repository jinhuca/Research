namespace Crystal.Policy
{
  public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
}
