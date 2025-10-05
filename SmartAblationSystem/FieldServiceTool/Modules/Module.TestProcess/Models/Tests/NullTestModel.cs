using Module.Infrastructure;
using Module.Infrastructure.TestEntities;
using Module.Infrastructure.TestInterfaces;
using Module.Infrastructure.TestResults.Interfaces;
using Prism.Mvvm;
using System.Threading;
using System.Threading.Tasks;

namespace Module.TestProcess.Models.Tests
{
	public class NullTestModel : BindableBase, ITestModel
	{
		private ITestInfo _info;
		public ITestInfo Info
		{
			get => _info;
			set => SetProperty(ref _info, value);
		}

		public NullTestModel(ITestInfo testInfo)
		{
			Info = testInfo;
			Info.Entity = TestEntity.NullTestEntity;
		}

		public async Task<ITestInfo> Start(CancellationToken cancellationToken, ISessionModel sessionModel)
		{
			if(cancellationToken.IsCancellationRequested)
			{
				Info.Status = TestStatus.Aborted;
			}
			return await Task.FromResult(Info);
		}

		public void Stop() => Info.Status = TestStatus.Stopped;
		public void Pause() => Info.Status = TestStatus.Paused;
		public void Resume() => Info.Status = TestStatus.Inprogress;
	}
}
