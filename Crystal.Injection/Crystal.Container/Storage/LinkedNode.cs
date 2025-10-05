using System.Diagnostics;

namespace Crystal.Storage
{
  [DebuggerDisplay("Node:  Key={Key},  Value={Value}")]
  public class LinkedNode<TKey, TValue>
  {
    public TKey Key;
    public TValue Value;
    public LinkedNode<TKey, TValue> Next;
  }
}
