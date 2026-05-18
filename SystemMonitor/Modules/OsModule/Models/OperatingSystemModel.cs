using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SystemManagementProvider.Constants;
using SystemManagementProvider.Interfaces;

namespace OsModule.Models; 
public class OperatingSystemModel : BindableBase, IOperatingSystemModel {
  private readonly ISMProvider? _smProvider;

  public OperatingSystemModel(ISMProvider? smProvider) {
    _smProvider = smProvider;
    //ISMQuery osQuery_ = _smProvider.GetQueryProvider(SMCategories.OperatingSystem);
    //var osInfoDict = osQuery_.Query(Win32_OperatingSystem.QueryString);
    fetchSystemInfo(null);
  }

  private void fetchSystemInfo(object? state) {
    ISMQuery? osQuery_ = _smProvider?.GetQueryProvider(SMCategories.OperatingSystem);
    var osInfoDict = osQuery_?.Query(Win32_OperatingSystem.QueryString);
    if (osInfoDict != null) {
      Caption = osInfoDict[Win32_OperatingSystem.CaptionKey].Item1;
      BuildNumber = osInfoDict[Win32_OperatingSystem.BuildNumberKey].Item1;
      Version = osInfoDict[Win32_OperatingSystem.VersionKey].Item1;
      Language = osInfoDict[Win32_OperatingSystem.OSLanguageKey].Item1;
      OSArchitecture = osInfoDict[Win32_OperatingSystem.OSArchitectureKey].Item1;
      CodeSet = osInfoDict[Win32_OperatingSystem.CodeSetKey].Item1;
      CSName = osInfoDict[Win32_OperatingSystem.CSNameKey].Item1;
      TimeZone = osInfoDict[Win32_OperatingSystem.CurrentTimeZoneKey].Item1;
      SerialNumber = osInfoDict[Win32_OperatingSystem.SerialNumberKey].Item1;
      Locale = osInfoDict[Win32_OperatingSystem.LocaleKey].Item1;
    }
  }

  private string _caption = string.Empty;
  public string Caption {
    get => _caption;
    set => SetProperty(ref _caption, value);
  }

  private string _buildNumber = string.Empty;
  public string BuildNumber {
    get => _buildNumber;
    set => SetProperty(ref _buildNumber, value);
  }

  private string _version = string.Empty;
  public string Version {
    get => _version;
    set => SetProperty(ref _version, value);
  }

  private string _language = string.Empty;
  public string Language {
    get => _language;
    set => SetProperty(ref _language, value);
  }

  private string _osArchitecture = string.Empty;
  public string OSArchitecture {
    get => _osArchitecture;
    set => SetProperty(ref _osArchitecture, value);
  }

  private string _codeSet = string.Empty;
  public string CodeSet {
    get => _codeSet;
    set => SetProperty(ref _codeSet, value);
  }

  private string _csname = string.Empty;
  public string CSName {
    get => _csname;
    set => SetProperty(ref _csname, value);
  }

  private string _timeZone = string.Empty;
  public string TimeZone {
    get => _timeZone;
    set => SetProperty(ref _timeZone, value);
  }

  private string _serialNumber = string.Empty;
  public string SerialNumber {
    get => _serialNumber;
    set => SetProperty(ref _serialNumber, value);
  }

  private string _locale = string.Empty;
  public string Locale {
    get => _locale;
    set => SetProperty(ref _locale, value);
  }

  public event PropertyChangedEventHandler? PropertyChanged;
  public event NotifyCollectionChangedEventHandler? CollectionChanged;
}
