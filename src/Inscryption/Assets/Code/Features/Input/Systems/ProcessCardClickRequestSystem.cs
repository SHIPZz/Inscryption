using Entitas;

namespace Code.Features.Input.Systems
{
    public class ProcessCardClickRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _selectedCards;
        private readonly IGroup<GameEntity> _activeHeroes;

        public ProcessCardClickRequestSystem(GameContext game)
        {
            _game = game;
            _requests = game.GetGroup(GameMatcher.CardClickRequest);
            _selectedCards = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Selected));
            _activeHeroes = _game.GetGroup(GameMatcher.AllOf(GameMatcher.Hero, GameMatcher.HeroTurn));
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests)
            {
                int cardId = request.CardClickRequest;
                GameEntity card = _game.GetEntityWithId(cardId);

                foreach (var activeHero in _activeHeroes)
                {
                    if (card != null && card.isCard && card.hasCardOwner && card.CardOwner == activeHero.Id)
                    {
                        bool wasSelected = card.isSelected;

                        DeselectAllCards();

                        if (!wasSelected)
                            card.isSelected = true;
                    }
                }
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

