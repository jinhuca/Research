using Module.Infrastructure;
using Module.Infrastructure.Constants;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using static Module.Infrastructure.StepStatus;

namespace Module.TestProcess.Models.Steps
{
	public class NullStepModel : BindableBase, IStepModel
	{
		private StepEntity _Entity = StepEntity.NullStep;
		public StepEntity Entity
		{
			get => _Entity;
			set => SetProperty(ref _Entity, value);
		}

		private StepStatus _Status = Unknown;
		public StepStatus Status
		{
			get => _Status;
			set => SetProperty(ref _Status, value);
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
			set=>SetProperty(ref _PassedPercentage, value);
		}

		private Dictionary<TestId, ITestModel> _Tests = new Dictionary<TestId, ITestModel>();
		public Dictionary<TestId, ITestModel> Tests
		{
			get => _Tests; 
			set => SetProperty(ref _Tests, value);
		}
		public DateTime? StartTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public DateTime? FinishTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
