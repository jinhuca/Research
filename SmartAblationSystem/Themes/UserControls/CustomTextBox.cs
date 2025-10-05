using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace CustomControls.UserControls
{
  public class CustomTextBox : TextBox
  {
    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new FrameworkElementAutomationPeer(this);
    }
  }
}
