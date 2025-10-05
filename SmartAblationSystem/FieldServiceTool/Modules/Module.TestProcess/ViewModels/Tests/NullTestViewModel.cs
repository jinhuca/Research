using Module.Infrastructure;
using Module.Infrastructure.TestEntities;
using Prism.Mvvm;

namespace Module.TestProcess.ViewModels.Tests
{
	public class NullTestViewModel : BindableBase, ITestViewModel
	{
		private TestEntity _Entity = TestEntity.NullTestEntity;
		public TestEntity Entity
		{
			get => _Entity;
			set => SetProperty(ref _Entity, value);
		}

		private TestStatus _Status = TestStatus.Unknown;
		public TestStatus Status
		{
			get => _Status;
			set => SetProperty(ref _Status, value);
		}
	}
}
