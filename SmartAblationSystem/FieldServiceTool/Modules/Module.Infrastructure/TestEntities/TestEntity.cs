using Module.Infrastructure.Constants;
using Prism.Mvvm;

namespace Module.Infrastructure.TestEntities
{
	public class TestEntity : BindableBase
	{
		private TestId _Id = TestId.Unknown;
		public TestId Id
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

		private StepId _StepId = StepId.Unknown;
		public StepId StepId
		{
			get => _StepId;
			set => SetProperty(ref _StepId, value);
		}

		public static TestEntity NullTestEntity = new TestEntity
		{
			Id = TestId.Unknown,
			Title = string.Empty,
			Description = string.Empty,
			StepId = StepId.Unknown
		};

		public static TestEntity VersionVerificationEntity = new TestEntity
		{
			Id = TestId.VersionVerification,
			Title = "Version Verification",
			Description = "Step 1: Manual Tests - Version Verification.",
			StepId = StepEntity.Step1.Id
		};

		public static TestEntity InputTestEntity = new TestEntity
		{
			Id = TestId.InputTest,
			Title = "Input Test",
			Description = "Input Test.",
			StepId = StepEntity.Step1.Id
		};

		public static TestEntity VisualTestEntity = new TestEntity
		{
			Id = TestId.VisualTest,
			Title = "Visual Test",
			Description = "Visual Test: Verify the System LEDs are functional (illuminated or flashing) ...",
			StepId = StepEntity.Step1.Id
		};

		public static TestEntity AudibleTestEntity = new TestEntity
		{
			Id = TestId.AudibleTest,
			Title = "Audible Test",
			Description = "Audio Test: verify the system speaker can be heard ...",
			StepId = StepEntity.Step1.Id
		};

		public static TestEntity IdleStateCheckEntity = new TestEntity
		{
			Id = TestId.IdleStateCheck,
			Title = "Idle State",
			Description = "Idle State Check",
			StepId = StepEntity.Step2.Id
		};

		public static TestEntity ReadyStateCheckEntity = new TestEntity
		{
			Id = TestId.ReadyStateCheck,
			Title = "Ready State",
			Description = "Ready State Readiness Check.",
			StepId = StepEntity.Step2.Id
		};

		public static TestEntity AblationTestsEntity = new TestEntity
		{
			Id = TestId.AblationTests,
			Title = "Ablation Tests",
			Description = "Ablation Tests.",
			StepId = StepEntity.Step3.Id
		};

		public static TestEntity DMSTestEntity = new TestEntity
		{
			Id = TestId.DMSTests,
			Title = "DMS Tests",
			Description = "DMS Tests.",
			StepId = StepEntity.Step3.Id
		};

		public static TestEntity ETSTestEntity = new TestEntity
		{
			Id = TestId.ETSTests,
			Title = "ETS Tests",
			Description = "ETS Tests.",
			StepId = StepEntity.Step3.Id
		};

		public static TestEntity OPSTestEntity = new TestEntity
		{
			Id = TestId.OPSTests,
			Title = "OPS Tests",
			Description = "OPS Tests.",
			StepId = StepEntity.Step3.Id
		};
	}
}
