using Crystal.Builder;
using Crystal.Processors;
using Crystal.Storage;

namespace Crystal.Extension
{
  public class Legacy : CrystalContainerExtension
  {
    protected override void Initialize()
    {
      var strategies = (StagedStrategyChain<MemberProcessor, BuilderStage>)Context.BuildPlanStrategies;
      var processor = (ConstructorProcessor)strategies.First(s => s is ConstructorProcessor);

      processor.SelectMethod = processor.LegacySelector;
    }
  }
}
