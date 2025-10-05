using Module.Infrastructure.Constants;
using Prism.Mvvm;

namespace Module.Infrastructure.TestEntities
{
	public class StepEntity : BindableBase
	{
		private StepId _Id = StepId.Unknown;
		public StepId Id
		{
			get => _Id;
			set => SetProperty(ref _Id, value);
		}

		private string _Title = string.Empty;
		public string Title
		{
			get => _Title;
			set => SetProperty(ref _Title, value);
		}

		private string _Description = string.Empty;
		public string Description
		{
			get => _Description;
			set => SetProperty(ref _Description, value);
		}

		public static StepEntity NullStepEntity = new StepEntity
		{
			Id = StepId.Unknown,
			Title = string.Empty,
			Description = string.Empty
		};

		public static StepEntity NullStep { get; } = new StepEntity
		{
			Id = StepId.Unknown,
			Title = string.Empty,
			Description = string.Empty
		};

		public static StepEntity Step1 { get; } = new StepEntity
		{
			Id = StepId.Step1,
			Title = "Step 1",
			Description = "Manual Tests"
		};

		public static StepEntity Step2 { get; } = new StepEntity
		{
			Id = StepId.Step2,
			Title = "Step 2",
			Description = "Parameter Check"
		};

		public static StepEntity Step3 { get; } = new StepEntity
		{
			Id = StepId.Step3,
			Title = "Step 3",
			Description = "Performance Tests"
		};
	}
}
