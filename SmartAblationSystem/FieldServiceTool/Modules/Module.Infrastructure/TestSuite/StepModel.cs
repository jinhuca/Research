using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Module.Infrastructure.TestSuite
{
	public class StepModel : IStepModel
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public double ProcessedPercentage { get; set; }
		public double PassedPercentage { get; set; }
		public IDictionary<string, ITestModel> Tests { get; set; } = new Dictionary<string, ITestModel>();
	}
}
