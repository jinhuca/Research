using SmartAblationSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartAblationSystem.Views
{
	/// <summary>
	/// Interaction logic for ActionLog.xaml.
	/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	public partial class ActionLog : UserControl
	{
		/// <summary>
		/// Initializes Action Log components.
		/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
		/// </summary>
		public ActionLog()
		{
			InitializeComponent();
		}

		private void ActionLog_OnLoaded(object sender, RoutedEventArgs e)
		{
			var dc_ = (ActionLogViewModel)DataContext;
			if(dc_ != null)
			{
				dc_.ActionLog = CommonViewModel.Current.ActionLog;
			}
		}

		private void ActionLog_OnUnloaded(object sender, RoutedEventArgs e)
		{
			var dc_ = (ActionLogViewModel)DataContext;
			if(dc_ != null)
			{
				dc_.ActionLog = null;
			}
		}
	}
}