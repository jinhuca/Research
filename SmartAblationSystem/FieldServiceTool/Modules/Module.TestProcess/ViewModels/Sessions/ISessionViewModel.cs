using Module.Infrastructure;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Module.TestProcess.ViewModels.Sessions
{
	public interface ISessionViewModel : INotifyPropertyChanged
	{
		string Id { get; set; }
		SessionStatus Status { get; set; }
		ObservableCollection<IStepViewModel> StepViewModelCollection { get; set; }
	}
}
