using System;
using System.Threading;
using SmartAblationSystem.ViewModels;

namespace SmartAblationSystem.Views
{
  public partial class FileExportCancellationPopup
  {
    public FileExportCancellationPopup(CancellationTokenSource cancellation, IDataExportable context)
    {
      InitializeComponent();
      DataContext = new FileExportCancellationPopupViewModel(cancellation, context);
      ((FileExportCancellationPopupViewModel)DataContext).CloseWindowRequested += CloseWindow;
    }

    private void CloseWindow(object sender, EventArgs e)
    {
      ((FileExportCancellationPopupViewModel)DataContext).CloseWindowRequested -= CloseWindow;
      ((FileExportCancellationPopupViewModel)DataContext).ErrorMessage = null;
      Close();
    }
  }
}
