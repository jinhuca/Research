using Module.Infrastructure;
using Module.Infrastructure.TestEntities;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using static Module.Infrastructure.StepStatus;

namespace Module.TestProcess.ViewModels.Steps
{
	public class NullStepViewModel : BindableBase, IStepViewModel
	{
		private StepEntity _Entity = StepEntity.NullStep;
		public StepEntity Entity
		{
			get => _Entity; 
			set =>SetProperty(ref _Entity, value);
		}

		private StepStatus _Status = Unknown;
		public StepStatus Status
		{
			get=>_Status; 
			set=>SetProperty(ref _Status, value);
		}

		private double _ProcessedPercentage;
		public double ProcessedPercentage
		{
			get=> _ProcessedPercentage; 
			set =>SetProperty(ref _ProcessedPercentage, value);
		}

		private double _PassedPercentage;
		public double PassedPercentage
		{
			get =>_PassedPercentage;
			set => SetProperty(ref _PassedPercentage, value);
		}


		private ObservableCollection<ITestViewModel> _TestViewModels = new ObservableCollection<ITestViewModel>();
		public ObservableCollection<ITestViewModel> TestViewModels
		{
			get => _TestViewModels;
			set => SetProperty(ref _TestViewModels, value);
		}
	}
}
