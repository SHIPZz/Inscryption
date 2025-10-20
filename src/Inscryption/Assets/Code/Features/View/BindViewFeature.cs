using Code.Features.View.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.View
{
    public sealed class BindViewFeature : Feature
    {
        public BindViewFeature(ISystemFactory systems)
        {
            Add(systems.Create<BindEntityViewFromPathSystem>());
            Add(systems.Create<BindEntityViewFromPrefabSystem>());
            Add(systems.Create<BindEntityViewFromAssetReferenceSystem>());
            Add(systems.Create<BindEntityViewFromAddressableKeySystem>());
            
            Add(systems.Create<ProcessSelfInitializedEntityViewRequestsSystem>());
        }
    }
}