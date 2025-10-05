using System;

namespace Module.Infrastructure
{
	[Flags]
	public enum StepStatus
	{
		Unknown = 1,
		NotStarted = 2,
		InProgress = 4,
		Processed = 8,
		Finished = 16,
		Failed = 32,
		Passed = 64,
		FailedInProgress = InProgress | Failed,
		FailedFinished = Failed | Finished
	}
}
