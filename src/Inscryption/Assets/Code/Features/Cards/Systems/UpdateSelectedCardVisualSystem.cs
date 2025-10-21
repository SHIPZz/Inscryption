using System.Collections.Generic;
using Entitas;

namespace Code.Features.Cards.Systems
{
    public class UpdateSelectedCardVisualSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly HashSet<int> _previousSelectedCardIds = new();
        private readonly HashSet<int> _currentSelectedCardIds = new();

        public UpdateSelectedCardVisualSystem(GameContext game)
        {
            _game = game;
        }

        public void Execute()
        {
            _currentSelectedCardIds.Clear();

            if (_game.hasSelectedCards && _game.SelectedCards != null)
            {
                foreach (int cardId in _game.SelectedCards)
                {
                    _currentSelectedCardIds.Add(cardId);
                }
            }

            ProcessDeselectedCards();
            ProcessSelectedCards();

            UpdatePreviousSelection();
        }

        private void ProcessDeselectedCards()
        {
            foreach (int cardId in _previousSelectedCardIds)
            {
                if (!_currentSelectedCardIds.Contains(cardId))
                {
                    UpdateCardVisual(cardId, isSelected: false);
                }
            }
        }

        private void ProcessSelectedCards()
        {
            foreach (int cardId in _currentSelectedCardIds)
            {
                if (!_previousSelectedCardIds.Contains(cardId))
                {
                    UpdateCardVisual(cardId, isSelected: true);
                }
            }
        }

        private void UpdateCardVisual(int cardId, bool isSelected)
        {
            GameEntity card = _game.GetEntityWithId(cardId);

            if (card != null && card.hasCardAnimator)
            {
                if (isSelected)
                    card.CardAnimator.Select();
                else
                    card.CardAnimator.Deselect();
            }
        }

        private void UpdatePreviousSelection()
        {
            _previousSelectedCardIds.Clear();
            _previousSelectedCardIds.UnionWith(_currentSelectedCardIds);
        }
    }
}

