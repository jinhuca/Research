using Module.Infrastructure.AppLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Module.Infrastructure.Helpers
{
	public static class ThreadHelpers
	{
		/// <summary>
		/// Wait synchronously for period specified by <paramref name="seconds"/> seconds.
		/// </summary>
		/// <param name="seconds">Wait period in second.</param>
		public static void WaitFor(double seconds = 1.0d)
		{
			Task.Delay(TimeSpan.FromSeconds(seconds)).Wait();
		}

		/// <summary>
		/// Wait synchronously for period specified by <paramref name="seconds"/> seconds
		/// with cancellation token <paramref name="cancellationToken"/>.
		/// </summary>
		/// <param name="seconds">Wait period in second.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public static void WaitFor(CancellationToken cancellationToken, double seconds = 1.0d)
		{
			try
			{
				Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken).Wait();
			}
			catch(Exception ex) when(ex.InnerException is TaskCanceledException)
			{
				FieldServiceTrace.Log(ex.InnerException.Message);
			}
		}

		/// <summary>
		/// Wait asynchronously for period specified by <paramref name="seconds"/> seconds.
		/// </summary>
		/// <param name="seconds">Wait period in second.</param>
		/// <returns>Object from Task Delay.</returns>
		public static async Task WaitForAsync(double seconds)
		{
			await Task.Delay(TimeSpan.FromSeconds(seconds));
		}

		/// <summary>
		/// Wait asynchronously for period specified by <paramref name="seconds"/> seconds
		/// with cancellation token <paramref name="cancellationToken"/>.
		/// </summary>
		/// <param name="seconds">Wait period in second.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Object from Task.Delay.</returns>
		public static async Task WaitForAsync(double seconds, CancellationToken cancellationToken)
		{
			try
			{
				await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
			}
			catch(TaskCanceledException ex)
			{
				FieldServiceTrace.Log(ex.Message);
			}
		}

		/// <summary>
		/// Wait for period specified by <paramref name="seconds"/> seconds.
		/// </summary>
		/// <param name="seconds">Wait period in second.</param>
		public static void CountdownInSecond(double seconds)
		{
			using (var cde = new CountdownEvent(1))
			{
				cde.Wait(TimeSpan.FromSeconds(seconds));
			}
		}

		/// <summary>
		/// Wait for period specified by <paramref name="seconds"/> seconds with cancellation token
		/// <paramref name="cancellationToken"/>
		/// </summary>
		/// <param name="seconds">Wait period in second.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public static void CountdownWithCancellationInSecond(double seconds, CancellationToken cancellationToken)
		{
			using (var cde = new CountdownEvent(1))
			{
				try
				{
					cde.Wait(TimeSpan.FromSeconds(seconds), cancellationToken);
				}
				catch (OperationCanceledException ex)
				{
					FieldServiceTrace.Log(ex.Message);
				}
			}
		}
	}
}
