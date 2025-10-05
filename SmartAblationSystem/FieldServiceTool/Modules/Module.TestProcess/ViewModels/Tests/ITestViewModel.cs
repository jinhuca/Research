using Module.Infrastructure;
using Module.Infrastructure.TestEntities;
using System.ComponentModel;

namespace Module.TestProcess.ViewModels
{
	public interface ITestViewModel : INotifyPropertyChanged
	{
		TestEntity Entity { get; set; }
		TestStatus Status { get; set; }
	}
}
