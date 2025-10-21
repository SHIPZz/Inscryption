using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Input.Systems
{
    public class ProcessCardClickRequestSystem : IExecuteSystem
    {
        private readonly InputContext _input;
        private readonly GameContext _game;
        private readonly IGroup<InputEntity> _requests;
        private readonly List<InputEntity> _buffer = new List<InputEntity>();

        public ProcessCardClickRequestSystem(InputContext input, GameContext game)
        {
            _input = input;
            _game = game;
            _requests = _input.GetGroup(InputMatcher.CardClickRequest);
        }

        public void Execute()
        {
            foreach (InputEntity request in _requests.GetEntities(_buffer))
            {
                int cardId = request.CardClickRequest;
                GameEntity card = _game.GetEntityWithId(cardId);

                if (card is { isCard: true, isHeroOwner: true })
                {
                    ToggleCardSelection(cardId);
                }

                request.Destroy();
            }
        }

        private void ToggleCardSelection(int cardId)
        {
            if (!_game.hasSelectedCards)
            {
                _game.ReplaceSelectedCards(new List<int> { cardId });
                Debug.Log($"[ProcessCardClickRequest] Card {cardId} selected");
                return;
            }

            List<int> selectedCards = _game.SelectedCards;
            int cardIndex = FindCardIndex(selectedCards, cardId);

            if (cardIndex != -1)
            {
                RemoveCardAtIndex(selectedCards, cardIndex);
                Debug.Log($"[ProcessCardClickRequest] Card {cardId} deselected");
            }
            else
            {
                selectedCards.Add(cardId);
                _game.ReplaceSelectedCards(selectedCards);
                Debug.Log($"[ProcessCardClickRequest] Card {cardId} selected");
            }
        }

        private int FindCardIndex(List<int> cards, int cardId)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] == cardId)
                    return i;
            }
            return -1;
        }

        private void RemoveCardAtIndex(List<int> selectedCards, int index)
        {
            selectedCards.RemoveAt(index);

            if (selectedCards.Count == 0)
                _game.RemoveSelectedCards();
            else
                _game.ReplaceSelectedCards(selectedCards);
        }
    }
}

