using Module.CatheterTestTool.Models;
using Module.CatheterTestTool.Services;
using Module.CatheterTestTool.ViewModels;
using Module.CatheterTestTool.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace Module.CatheterTestTool
{
	public class CatheterTestToolModule : IModule
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterSingleton<CatheterTestMainWindowViewModel>();
			containerRegistry.RegisterSingleton<CatheterInfoData>();
      containerRegistry.Register<ITestDataFileManager, TestDataFileManager>();

      containerRegistry.RegisterDialog<CatheterTestPopupMessageView, CatheterTestPopupMessageViewModel>();
      containerRegistry.RegisterDialog<TestResultFileSelector, TestResultFileSelectorViewModel>();
      containerRegistry.RegisterDialog<CatheterTestResultDialog, CatheterTestResultDialogViewModel>();
		}

		public void OnInitialized(IContainerProvider containerProvider)
		{
			
		}
	}
}
