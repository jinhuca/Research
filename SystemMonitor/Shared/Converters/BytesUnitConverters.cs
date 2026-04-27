namespace Converters;

public class ByteUnitConverters {
  public static string ConvertBytesToReadableUnit(long bytes) {
    string[] units = { "B", "KB", "MB", "GB", "TB" };
    double size = bytes;
    int unitIndex = 0;

    if (bytes < 0) {
      throw new ArgumentOutOfRangeException(nameof(bytes), "Bytes cannot be negative.");
    }

    if(bytes < 1024) {
      return $"{bytes} B";
    }
    
    while (size >= 1024 && unitIndex < units.Length - 1) {
      size /= 1024;
      unitIndex++;
    }

    return $"{size:F2} {units[unitIndex]}";
  }
}
