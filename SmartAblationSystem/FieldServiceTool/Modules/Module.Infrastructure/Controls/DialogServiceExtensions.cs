using System;
using Prism.Services.Dialogs;

namespace Module.Infrastructure.Controls
{
	public static class DialogServiceExtensions
	{
		public static void ShowNotification(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
		{
			dialogService.ShowDialog(nameof(Dialog), new DialogParameters($"message={message}"), callBack, "Dialog");
		}
	}
}
