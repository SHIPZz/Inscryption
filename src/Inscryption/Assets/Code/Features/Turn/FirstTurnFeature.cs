using Code.Features.Cards;
using Code.Features.Game.Systems;
using Code.Features.Movement;
using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class FirstTurnFeature : Feature
    {
        public FirstTurnFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<InitializeGameSystem>());
            Add(systemFactory.Create<CardFeature>());
            Add(systemFactory.Create<MovementFeature>());
            Add(systemFactory.Create<SetHeroTurnSystem>());
            Add(systemFactory.Create<TransitionToPlacementSystem>());
        }
    }
}

