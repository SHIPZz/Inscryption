using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace Code.Features.Cards.Systems
{
    public class UpdateSelectedCardVisualSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly HashSet<int> _previousSelectedCardIds = new HashSet<int>();

        public UpdateSelectedCardVisualSystem(GameContext game)
        {
            _game = game;
        }

        public void Execute()
        {
            HashSet<int> currentSelectedCardIds = new HashSet<int>();
            
            if (_game.hasSelectedCards && _game.SelectedCards != null)
            {
                currentSelectedCardIds = new HashSet<int>(_game.SelectedCards);
            }

            foreach (int cardId in _previousSelectedCardIds)
            {
                if (!currentSelectedCardIds.Contains(cardId))
                {
                    GameEntity card = _game.GetEntityWithId(cardId);
                    if (card != null && card.hasCardAnimator)
                    {
                        card.CardAnimator.Deselect();
                    }
                }
            }

            foreach (int cardId in currentSelectedCardIds)
            {
                if (!_previousSelectedCardIds.Contains(cardId))
                {
                    GameEntity card = _game.GetEntityWithId(cardId);
                    if (card != null && card.hasCardAnimator)
                    {
                        card.CardAnimator.Select();
                    }
                }
            }

            _previousSelectedCardIds.Clear();
            _previousSelectedCardIds.UnionWith(currentSelectedCardIds);
        }
    }
}

