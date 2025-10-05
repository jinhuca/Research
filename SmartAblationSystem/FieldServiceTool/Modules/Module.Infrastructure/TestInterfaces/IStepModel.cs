using Module.Infrastructure.Constants;
using Module.Infrastructure.TestEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Module.Infrastructure.TestInterfaces
{
	public interface IStepModel : INotifyPropertyChanged
	{
		StepEntity Entity { get; set; }
		StepStatus Status { get; set; }
		DateTime? StartTime { get; set; }
		DateTime? FinishTime { get; set; }
		double ProcessedPercentage { get; set; }
		double PassedPercentage { get; set; }
		Dictionary<TestId, ITestModel> Tests { get; set; }
	}
}
