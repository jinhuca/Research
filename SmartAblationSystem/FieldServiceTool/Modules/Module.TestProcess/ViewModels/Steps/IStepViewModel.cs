using Module.Infrastructure;
using Module.Infrastructure.TestEntities;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Module.TestProcess.ViewModels
{
	public interface IStepViewModel : INotifyPropertyChanged
	{
		StepEntity Entity { get; set; }
		StepStatus Status { get; set; }
		double ProcessedPercentage { get; set; }
		double PassedPercentage { get; set; }
		ObservableCollection<ITestViewModel> TestViewModels { get; set; }
	}
}
