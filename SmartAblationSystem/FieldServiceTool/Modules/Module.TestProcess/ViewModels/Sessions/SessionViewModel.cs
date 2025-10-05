using Module.Infrastructure;
using Module.Infrastructure.TestInterfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Unity;
using static Module.Infrastructure.SessionStatus;

namespace Module.TestProcess.ViewModels.Sessions
{
	public class SessionViewModel : BindableBase, ISessionViewModel
	{
		private string _Id = string.Empty;
		public string Id
		{
			get => _Id;
			set => SetProperty(ref _Id, value);
		}

		private SessionStatus _Status = Unknown;
		public SessionStatus Status
		{
			get => _Status;
			set => SetProperty(ref _Status, value);
		}

		private ObservableCollection<IStepViewModel> _StepViewModelCollection = new ObservableCollection<IStepViewModel>();
		public ObservableCollection<IStepViewModel> StepViewModelCollection
		{
			get => _StepViewModelCollection;
			set => SetProperty(ref _StepViewModelCollection, value);
		}

		public SessionViewModel(IUnityContainer container, ISessionModel sessionModel)
		{
			Id = sessionModel.Id;
			Status = sessionModel.Status;

			foreach (var stepModel in sessionModel.Steps)
			{
				var stepViewModel_ = container.Resolve<IStepViewModel>();
				stepViewModel_.Entity = stepModel.Value.Entity;
				stepViewModel_.Status = stepModel.Value.Status;
				foreach (var testModel in stepModel.Value.Tests)
				{
					var testViewModel_ = container.Resolve<ITestViewModel>();
					testViewModel_.Entity = testModel.Value.Info.Entity;
					testViewModel_.Status = testModel.Value.Info.Status;
					stepViewModel_.TestViewModels.Add(testViewModel_);
				}
				StepViewModelCollection.Add(stepViewModel_);
			}
		}
	}
}
