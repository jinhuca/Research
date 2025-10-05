using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Module.Infrastructure.Constants
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum TestId
	{
		VersionVerification,
		InputTest,
		VisualTest,
		AudibleTest,
		IdleStateCheck,
		ReadyStateCheck,
		AblationTests,
		DMSTests,
		ETSTests,
		OPSTests,
		Unknown
	}
}
