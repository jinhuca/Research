using System.Windows.Controls;

namespace MainApp.Contents; 

public partial class CpuLoadView : UserControl {
  public CpuLoadView() {
    InitializeComponent();
  }

  override protected void OnInitialized(EventArgs e) {
    base.OnInitialized(e);
    CpuInfoServices.Observables.CpuInfoGenerators.GenerateCpuLiveInfo(TimeSpan.FromSeconds(1))
      .Subscribe(info => {
        //CpuLoadText.Text = $"CPU Load: {info.LoadPercentage}%";
      });
  }
}
