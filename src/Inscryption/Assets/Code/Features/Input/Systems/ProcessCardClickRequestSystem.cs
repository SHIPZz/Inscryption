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

        public ProcessCardClickRequestSystem(InputContext input, GameContext game)
        {
            _input = input;
            _game = game;
            _requests = _input.GetGroup(InputMatcher.CardClickRequest);
        }

        public void Execute()
        {
            foreach (InputEntity request in _requests.GetEntities())
            {
                int cardId = request.CardClickRequest;
                GameEntity card = _game.GetEntityWithId(cardId);

                if (card is { isCard: true, isHeroOwner: true })
                {
                    if (!_game.hasSelectedCards)
                    {
                        _game.ReplaceSelectedCards(new List<int> { cardId });
                        Debug.Log($"[ProcessCardClickRequest] Card {cardId} selected");
                    }
                    else
                    {
                        List<int> selectedCards = _game.SelectedCards;
                        
                        if (selectedCards.Contains(cardId))
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
                        else
                        {
                            selectedCards.Add(cardId);
                            _game.ReplaceSelectedCards(selectedCards);
                            Debug.Log($"[ProcessCardClickRequest] Card {cardId} selected");
                        }
                    }
                }

                request.Destroy();
            }
        }
    }
}

