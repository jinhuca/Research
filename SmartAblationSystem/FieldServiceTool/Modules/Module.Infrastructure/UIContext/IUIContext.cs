using System;

namespace Module.Infrastructure.UIContext
{
	public interface IUIContext
	{
		bool IsSynchronized { get; }
		void Invoke(Action action);
		void BeginInvoke(Action action);
	}
}
