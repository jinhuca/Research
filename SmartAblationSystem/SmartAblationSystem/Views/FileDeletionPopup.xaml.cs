using SmartAblationSystem.ViewModels;
using System.Windows;

namespace SmartAblationSystem.Views
{
	public partial class FileDeletionPopup
	{
		private readonly IDataExportable _context;

		public FileDeletionPopup(IDataExportable context)
		{
			InitializeComponent();
			_context = context;
			DataContext = new FileDeletionViewModel(context);
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			_context?.OnDeleteDataFiles(true);
			DialogResult = true;
			Close();
		}

		private void No_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}