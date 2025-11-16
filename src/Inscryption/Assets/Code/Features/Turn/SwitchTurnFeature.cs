using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    //TODO REFACTOR THIS
    public class SwitchTurnFeature : Feature
    {
        public SwitchTurnFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<TurnFeature>());
            Add(systemFactory.Create<ClearCurrentPlayerTurnSystem>());
            Add(systemFactory.Create<TransitionToNextTurnSystem>());
            Add(systemFactory.Create<ProcessTurnTransitionSystem>());
        }
    }
}

