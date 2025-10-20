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
                SelectCard(cardId);
                return;
            }

            List<int> selectedCards = _game.SelectedCards;

            if (selectedCards.Contains(cardId))
            {
                DeselectCard(cardId, selectedCards);
            }
            else
            {
                AddToSelection(cardId, selectedCards);
            }
        }

        private void SelectCard(int cardId)
        {
            _game.ReplaceSelectedCards(new List<int> { cardId });
            Debug.Log($"[ProcessCardClickRequest] Card {cardId} selected");
        }

        private void DeselectCard(int cardId, List<int> selectedCards)
        {
            selectedCards.Remove(cardId);

            if (selectedCards.Count == 0)
            {
                _game.RemoveSelectedCards();
            }
            else
            {
                _game.ReplaceSelectedCards(selectedCards);
            }

            Debug.Log($"[ProcessCardClickRequest] Card {cardId} deselected");
        }

        private void AddToSelection(int cardId, List<int> selectedCards)
        {
            selectedCards.Add(cardId);
            _game.ReplaceSelectedCards(selectedCards);
            Debug.Log($"[ProcessCardClickRequest] Card {cardId} selected");
        }
    }
}

