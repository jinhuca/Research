using Console;
using Module.Console.Helpers;
using Module.Console.Interfaces;
using Prism.Mvvm;

namespace Module.Console.Models
{
	public partial class MachineModel : BindableBase, IMachineModel
	{
		private readonly ConsoleMonitor _consoleMonitor;

		public MachineModel(Machine machine, ConsoleMonitor consoleMonitor, Data data)
		{
			_machine = machine;
			_consoleMonitor = consoleMonitor;
			this.data = data;

			InitializeMachine();
			_consoleMonitor?.SetupCanBusCommunication();
		}
	}
}
