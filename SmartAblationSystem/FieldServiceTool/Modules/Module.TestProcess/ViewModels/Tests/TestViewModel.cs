using Module.Infrastructure;
using Module.Infrastructure.TestEntities;
using Prism.Mvvm;

namespace Module.TestProcess.ViewModels.Tests
{
	public class TestViewModel : BindableBase, ITestViewModel
	{
		private TestEntity _entity = TestEntity.NullTestEntity;
		public TestEntity Entity
		{
			get => _entity; 
			set => SetProperty(ref _entity, value);
		}

		private TestStatus _status = TestStatus.Unknown;
		public TestStatus Status
		{
			get => _status;
			set => SetProperty(ref _status, value);
		}
	}
}
