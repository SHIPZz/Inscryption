using Code.Features.Cards;
using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class EnemyPlaceCardsFeature : Feature
    {
        public EnemyPlaceCardsFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<PlaceEnemyCardsSystem>());
            Add(systemFactory.Create<CardFeature>());
            Add(systemFactory.Create<TransitionToAttackAfterPlacementSystem>());
        }
    }
}

