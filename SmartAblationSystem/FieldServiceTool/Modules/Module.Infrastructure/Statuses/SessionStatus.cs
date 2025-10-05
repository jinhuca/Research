namespace Module.Infrastructure
{
	public enum SessionStatus
	{
		Unknown,
		Ready,
		Starting,
		Started,
		Pausing,
		Paused,
		Resuming,
		Resumed,
		Stopping,
		Stopped,
		Finishing,
		Finished,
		Exception
	}
}
