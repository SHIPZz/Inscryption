using Code.Features.Cards;
using Code.Features.Input;
using Code.Features.Movement;
using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class PlacementFeature : Feature
    {
        public PlacementFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<InputFeature>());
            Add(systemFactory.Create<CardFeature>());
            Add(systemFactory.Create<MovementFeature>());
            Add(systemFactory.Create<ProcessEndTurnRequestSystem>());
            Add(systemFactory.Create<TransitionToAttackSystem>());
        }
    }
}

