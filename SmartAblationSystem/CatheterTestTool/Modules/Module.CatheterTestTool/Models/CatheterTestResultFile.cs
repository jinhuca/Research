
using Prism.Mvvm;

namespace Module.CatheterTestTool.Models
{
  public class CatheterTestResultFile : BindableBase
  {
    public CatheterTestResultFile(string fileName)
    {
      FileName = fileName;
      Selected = false;
    }

    private string _fileName;
    public string FileName { get => _fileName; set => SetProperty(ref _fileName, value); }

    private bool _selected;
    public bool Selected { get => _selected; set => SetProperty(ref _selected, value); }
  }
}
