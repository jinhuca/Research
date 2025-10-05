using Prism.Mvvm;

namespace SmartAblationSystem.ViewModels
{
	public class FileDeletionViewModel : BindableBase
	{
		public FileDeletionViewModel(IDataExportable context)
		{
			IsDeletingCurrentProcedure = context.IsExportingCurrentProcedure;
		}

		private bool _isDeletingCurrentProcedure;

		public bool IsDeletingCurrentProcedure
		{
			get => _isDeletingCurrentProcedure;
			set => SetProperty(ref _isDeletingCurrentProcedure, value);
		}
	}
}