using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Module.Infrastructure.Constants
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum StepId
	{
		Unknown,
		Step1,
		Step2,
		Step3
	}
}
