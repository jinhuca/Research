using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LogSystem
{
	public class LogItemBase
	{
		public long Id { get; set; }
		
		public DateTime Timestamp { get; set; }
		
		public int ThreadId { get; set; }
		
		public string ClassName { get; set; }
		
		public string MethodName { get; set; }
		
		public int LineNumber { get; set; }
		
		[JsonConverter(typeof(StringEnumConverter))]
		public LogLevel Level { get; set; }
		
		public string Info { get; set; }
	}
}
