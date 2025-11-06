using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class PlacementFeature : Feature
    {
        public PlacementFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<TransitionToAttackSystem>());
            Add(systemFactory.Create<ProcessEndTurnRequestSystem>());
        }
    }
}

