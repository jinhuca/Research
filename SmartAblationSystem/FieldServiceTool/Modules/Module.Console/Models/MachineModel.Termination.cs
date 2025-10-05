using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console;
using Prism.Ioc;

namespace Module.Console.Models
{
	/// <summary>
	/// Partial class for MachineModel - Termination.
	/// </summary>
	public partial class MachineModel
	{
		public async Task Terminate()
		{
			_machine.PowerOffMessage();
			await Task.Delay(500);
			_machine.DeactivateAllIOS();
			await Task.Delay(500);
			_machine.CanBusCommunication.Dispose();
			await Task.Delay(1_000);
		}
	}
}
