using Entitas;

namespace Code.Features.Turn.Systems
{
    public class SetHeroTurnSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _heroes;
        private bool _set;

        public SetHeroTurnSystem(GameContext game)
        {
            _heroes = game.GetGroup(GameMatcher.Hero);
        }

        public void Execute()
        {
            if (_set)
                return;

            foreach (GameEntity hero in _heroes)
            {
                if (hero.isHeroTurn)
                    return;

                hero.isHeroTurn = true;
                
                if (hero.hasCardsPlacedThisTurn)
                    hero.ReplaceCardsPlacedThisTurn(0);
                else
                    hero.AddCardsPlacedThisTurn(0);
                
                _set = true;
                break;
            }
        }
    }
}

