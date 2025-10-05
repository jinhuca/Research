namespace Shared
{
  public static class SharedConstants
  {
    public const string BalloonSize_28mm = @"28";
    public const string BalloonSize_31mm = @"31";

    public const double MIN_DASBalloonIBPSetPoint = 7.0;

    public static bool IsDasBalloonEnabledFromSetPoint(double pressureSetPoint) => pressureSetPoint > MIN_DASBalloonIBPSetPoint; 

    public static string BalloonSizeFromPressureSetPoint(double pressureSetPoint)
    {
      return pressureSetPoint < MIN_DASBalloonIBPSetPoint ? BalloonSize_28mm : BalloonSize_31mm;
    }
  }
}
