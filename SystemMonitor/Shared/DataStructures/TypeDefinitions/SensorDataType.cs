namespace DataStructures.TypeDefinitions;

public struct SensorDataType() {
  public float? Value { get; set; }
  public float? Min { get; set; }
  public float? Max { get; set; }

  //public static implicit operator SensorDataType((float? val_, float? min_, float? max_) v) {
  //  return new SensorDataType() { Value = v.val_, Min = v.min_, Max = v.max_ };
  //}
}
