namespace BiosModule.ViewModels.SubViewModels;

public interface ISM_SummaryViewModel {
  string Name { get; set; }
  string Manufacturer { get; set; }
  string SerialNum { get; set; }
  string MajorVersion { get; set; }
  string MinorVersion { get; set; }
}
