using System.Collections.Generic;
using Code.Features.Hero.Services;
using Entitas;

namespace Code.Features.Input.Systems
{
    public class ProcessCardClickRequestSystem : IExecuteSystem
    {
        private readonly InputContext _input;
        private readonly GameContext _game;
        private readonly IGroup<InputEntity> _requests;
        private readonly IGroup<GameEntity> _selectedCards;
        private readonly List<InputEntity> _buffer = new();
        private readonly IHeroProvider _heroProvider;

        public ProcessCardClickRequestSystem(InputContext input, GameContext game, IHeroProvider heroProvider)
        {
            _heroProvider = heroProvider;
            _input = input;
            _game = game;
            _requests = _input.GetGroup(InputMatcher.CardClickRequest);
            _selectedCards = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Selected));
        }

        public void Execute()
        {
            foreach (InputEntity request in _requests.GetEntities(_buffer))
            {
                int cardId = request.CardClickRequest;
                GameEntity card = _game.GetEntityWithId(cardId);

                GameEntity activeHero = _heroProvider.GetActiveHero();
                if (card != null && card.isCard && card.hasCardOwner && activeHero != null && card.CardOwner == activeHero.Id)
                {
                    bool wasSelected = card.isSelected;

                    DeselectAllCards();

                    if (!wasSelected)
                        card.isSelected = true;
                }

                request.Destroy();
            }
        }

        private void DeselectAllCards()
        {
            foreach (GameEntity selectedCard in _selectedCards.GetEntities())
            {
                selectedCard.isSelected = false;
            }
        }
    }
}

