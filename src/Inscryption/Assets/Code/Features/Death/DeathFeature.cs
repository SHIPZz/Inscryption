using Code.Features.Death.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Death
{
    public class DeathFeature : Feature
    {
        public DeathFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<MarkDeadSystem>());
            Add(systemFactory.Create<ClearSlotOnCardDestroyedSystem>());
        }
    }
}
