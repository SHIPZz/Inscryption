using System.Collections.Generic;
using Code.Features.Hero.Services;
using Entitas;

namespace Code.Features.Turn.Systems
{
    //todo refactor this
    public class ClearSelectionOnTurnEndSystem : IExecuteSystem
    {
        private readonly IHeroProvider _heroProvider;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _heroCards;
        private readonly List<GameEntity> _reqBuffer = new(4);
        private readonly List<GameEntity> _cardBuffer = new(16);

        public ClearSelectionOnTurnEndSystem(GameContext game, IHeroProvider heroProvider)
        {
            _heroProvider = heroProvider;
            _endTurnRequests = game.GetGroup(GameMatcher.AllOf(GameMatcher.EndTurnRequest, GameMatcher.ProcessingAvailable));
            _heroCards = game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Selected));
        }

        public void Execute()
        {
            foreach (GameEntity req in _endTurnRequests.GetEntities(_reqBuffer))
            {
                GameEntity hero = _heroProvider.GetHero();
                
                if (hero == null || !hero.isHeroTurn)
                    continue;

                foreach (GameEntity card in _heroCards.GetEntities(_cardBuffer))
                {
                    if (card.CardOwner == hero.Id)
                    {
                        card.isSelected = false;
                    }
                }
                // Important: do not destruct EndTurnRequest here; CreateAttacksOnEndTurnSystem will consume it
            }
        }
    }
}
