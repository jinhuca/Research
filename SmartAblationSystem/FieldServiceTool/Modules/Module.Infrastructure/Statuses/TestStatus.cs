using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Module.Infrastructure
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum TestStatus
	{
		Unknown,
		Aborted,
		NotStarted,
		Inprogress,
		Stopped,
		Paused,
		Passed,
		Failed,
		Finished,
		Retry
	}
}
