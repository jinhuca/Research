using System.Windows;

namespace ServiceToolApp.Views
{
	public partial class Shell
	{
		public Shell()
		{
			InitializeComponent();
#if DEBUG
			_debugErrorBtn.Visibility = Visibility.Visible;
#else
			_debugErrorBtn.Visibility = Visibility.Collapsed;
#endif
		}
	}
}
