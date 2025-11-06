using Code.Features.Cards;
using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class FirstTurnFeature : Feature
    {
        public FirstTurnFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<SetHeroTurnSystem>());
            Add(systemFactory.Create<TransitionToPlacementSystem>());
            Add(systemFactory.Create<CardFeature>());
        }
    }
}

