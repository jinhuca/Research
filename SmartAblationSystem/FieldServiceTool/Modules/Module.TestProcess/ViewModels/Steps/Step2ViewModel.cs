using System.Collections.Generic;
using Module.TestProcess.Entities;
using Module.TestProcess.Models.Steps;
using Prism.Mvvm;

namespace Module.TestProcess.ViewModels.Steps
{
	public class Step2ViewModel : BindableBase, IStepViewModel
	{
		public StepEntity Entity { get; set; }

		private bool _IsActive;
		public bool IsActive
		{
			get => _IsActive;
			set => SetProperty(ref _IsActive, value);
		}

		private double _ProcessedPercentage;
		public double ProcessedPercentage
		{
			get => _ProcessedPercentage;
			set => SetProperty(ref _ProcessedPercentage, value);
		}

		private double _PassedPercentage;
		public double PassedPercentage
		{
			get => _PassedPercentage;
			set => SetProperty(ref _PassedPercentage, value);
		}

		public List<ITestViewModel> TestViewModels { get; set; }

		public Step2ViewModel(Step2Model model)
		{
			Entity = model.Entity;
			ProcessedPercentage = model.ProcessedPercentage;
			PassedPercentage = model.PassedPercentage;
			TestViewModels = new List<ITestViewModel>();
		}
	}
}
