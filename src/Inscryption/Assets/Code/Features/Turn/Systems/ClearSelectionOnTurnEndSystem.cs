using System.Collections.Generic;
using Code.Features.Hero.Services;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class ClearSelectionOnTurnEndSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _heroSelectedCards;
        private readonly List<GameEntity> _cardBuffer = new(16);
        private readonly IGroup<GameEntity> _heroes;

        public ClearSelectionOnTurnEndSystem(GameContext game)
        {
            _endTurnRequests =
                game.GetGroup(GameMatcher.AllOf(GameMatcher.EndTurnRequest, GameMatcher.ProcessingAvailable));
            _heroSelectedCards =
                game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Selected, GameMatcher.HeroOwner));
            _heroes = game.GetGroup(GameMatcher.AllOf(GameMatcher.Hero, GameMatcher.HeroTurn));
        }

        public void Execute()
        {
            foreach (GameEntity request in _endTurnRequests)
            foreach (GameEntity hero in _heroes)
            {
                foreach (GameEntity card in _heroSelectedCards.GetEntities(_cardBuffer))
                {
                    card.isSelected = false;
                }
            }
        }
    }
}