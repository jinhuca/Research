using Module.Summary.Models;
using Module.Summary.Properties;
using Prism.Mvvm;

namespace Module.Summary.ViewModels
{
	public class SummaryViewModel : BindableBase
	{
		public SummaryViewModel(SummaryModel model)
		{
			_model = model;
			_model.PropertyChanged += _model_PropertyChanged;
		}

		private SummaryModel _model;
		public SummaryModel Model
		{
			get => _model;
			set => SetProperty(ref _model, value);
		}

		private void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
				case nameof(_model.Notes):
					Notes = _model.Notes;
					break;
			}
		}

		public string Title { get; } = Resources.ModuleTitle;

		private string _notes = string.Empty;
		public string Notes
		{
			get => _notes;
			set
			{
				if(_notes == value) return;
				SetProperty(ref _notes, value);
				_model.Notes = value;
			}
		}
	}
}
