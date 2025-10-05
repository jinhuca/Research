using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Module.Infrastructure.UIContext
{
	public sealed class WpfContext : IUIContext
	{
		private readonly Dispatcher _dispatcher;

		public WpfContext()
		{
			_dispatcher = Dispatcher.CurrentDispatcher;
		}

		public bool IsSynchronized => _dispatcher.Thread == Thread.CurrentThread;

		public void Invoke(Action action)
		{
			Debug.Assert(action!=null);
			_dispatcher.Invoke(action);
		}

		public void BeginInvoke(Action action)
		{
			Debug.Assert(action!=null);
			_dispatcher.BeginInvoke(action);
		}
	}
}
