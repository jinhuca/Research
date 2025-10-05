namespace Module.Infrastructure.TestSuite
{
	public interface ITestModel
	{
		string Title { get; set; }
		string Details { get; set; }
		TestStatus Status { get; set; }
		bool? Result { get; set; }
		object Data { get; set; }
		void Start();
		void Stop();
		void Pause();
		void Resume();
	}
}
