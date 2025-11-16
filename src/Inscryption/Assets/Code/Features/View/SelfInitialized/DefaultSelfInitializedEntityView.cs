using Code.Common;

namespace Code.Features.View.SelfInitialized
{
	public class DefaultSelfInitializedEntityView : AbstractSelfInitializedEntityView
	{
		protected override GameEntity BuildEntity() =>
			CreateEntity.Empty();
	}
}