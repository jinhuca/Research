using System.Collections.Generic;

namespace Module.CatheterTestTool.Configuration
{
  public class ValidationCriteria
  {
    public ValidationCriteria(string sensor, double minValue, double maxValue)
    {
      Sensor = sensor;
      MinValue = minValue;
      MaxValue = maxValue;
    }

    public string Sensor { get; }
    public double MinValue { get; }
    public double MaxValue { get; }
  }

  public class InflationIBPSetting
  {
    public InflationIBPSetting(double targetValue, double offset)
    {
      TargetValue = targetValue;
      Offset = offset;
    }

    public double TargetValue { get; }
    public double Offset { get; }
  }

  public interface ICatheterTestConfiguration
  {
    void LoadConfiguration(string configPath = null);
    IDictionary<string, ValidationCriteria> GetValidationCriteriaMap(int catheterId);
    InflationIBPSetting GetInflationIBPSetting(int catheterId);
  }
}