using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Console.Helpers
{
	public static class ETSdataSortingAndStatus
	{
		static List<int> lowestTemperatures = new List<int>();

		public static List<bool> ChannelStatus = new List<bool> { false, false, false, false,
			false, false,false, false,
			false, false,false, false,false};

		public static List<int> GetMin(List<double> sensorTemperatureList, out double Minimum)
		{
			int index = 0;
			lowestTemperatures.Clear();

			ChannelStatus.Clear();

			// fisrt we have to find the minium 
			if (sensorTemperatureList[0] == -100)
			{
				sensorTemperatureList[0] = int.MaxValue;
			}
			double min = sensorTemperatureList.Min();
			Minimum = min;

			if (min < 0 || min > 40)
			{
				min = int.MaxValue;
				Minimum = int.MaxValue;
			}


			//from sensorTemperature in sensorTemperatureList where sensorTemperature > 9 select sensorTemperature).Min();

			foreach (double sensor in sensorTemperatureList)
			{
				if (sensor == min)
				{
					lowestTemperatures.Add(index);
				}

				index++;

				if (sensor < 0 || sensor > 40)
				{

					ChannelStatus.Add(true);
				}
				else
				{

					ChannelStatus.Add(false);

				}
			}

			return lowestTemperatures;
		}

	}
}
