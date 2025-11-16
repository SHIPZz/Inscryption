using Entitas;

namespace Code.Features.Turn.Systems
{
    public class SetHeroTurnSystem : IInitializeSystem
    {
        private readonly IGroup<GameEntity> _heroes;

        public SetHeroTurnSystem(GameContext game)
        {
            _heroes = game.GetGroup(GameMatcher.Hero);
        }

        public void Initialize()
        {
            foreach (GameEntity hero in _heroes)
            {
                if (hero.isHeroTurn)
                    return;

                hero.isHeroTurn = true;
                
                if (hero.hasCardsPlacedThisTurn)
                    hero.ReplaceCardsPlacedThisTurn(0);
                else
                    hero.AddCardsPlacedThisTurn(0);
            }
        }
    }
}

