using System;

namespace LogSystem
{
	public class LogItemException : LogItemBase
	{
		public Exception ExceptionInstance { get; set; }
	}
}
