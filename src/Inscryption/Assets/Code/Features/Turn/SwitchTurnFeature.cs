using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class SwitchTurnFeature : Feature
    {
        public SwitchTurnFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<TransitionToNextTurnSystem>());
            Add(systemFactory.Create<ClearCurrentPlayerTurnSystem>());
        }
    }
}

