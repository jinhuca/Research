using Module.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Module.Infrastructure.TestInterfaces
{
	public interface ISessionModel : INotifyPropertyChanged
	{
		bool? Passed { get; set; }
		string Id { get; set; }
		SessionStatus Status { get; set; }
		DateTime? StartTime { get; set; }
		DateTime? FinishTime { get; set; }
		ManualResetEvent PauseResumeSignal { get; }
		Dictionary<StepId, IStepModel> Steps { get; set; }
		SessionStatus Start();
		SessionStatus Pause();
		SessionStatus Resume();
		SessionStatus Stop();
		void CreateSession();
	}
}
