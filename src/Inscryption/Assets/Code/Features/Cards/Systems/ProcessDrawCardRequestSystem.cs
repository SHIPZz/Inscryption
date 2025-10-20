using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class ProcessDrawCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _drawRequests;
        private readonly IGroup<GameEntity> _deckCards;
        private readonly IGroup<GameEntity> _playersWithHand;
        private readonly List<GameEntity> _buffer = new(8);

        private const int MaxHandSize = 5;

        public ProcessDrawCardRequestSystem(GameContext game)
        {
            _game = game;

            _drawRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.DrawCardRequest));

            _deckCards = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Card, GameMatcher.InHand)
                .NoneOf(GameMatcher.Destructed));

            _playersWithHand = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.CardsInHand));
        }

        public void Execute()
        {
            foreach (GameEntity request in _drawRequests.GetEntities(_buffer))
            {
                int playerId = request.drawCardRequest.PlayerId;
                GameEntity player = _game.GetEntityWithId(playerId);

                if (player == null)
                {
                    Debug.LogWarning($"[ProcessDrawCardRequestSystem] Player {playerId} not found!");
                    request.isDestructed = true;
                    continue;
                }

                if (!player.hasCardsInHand)
                {
                    Debug.LogWarning($"[ProcessDrawCardRequestSystem] Player {playerId} has no CardsInHand component!");
                    request.isDestructed = true;
                    continue;
                }

                if (player.CardsInHand.Count >= MaxHandSize)
                {
                    Debug.Log($"[ProcessDrawCardRequestSystem] Player {playerId} hand is full ({MaxHandSize} cards), skip draw");
                    request.isDestructed = true;
                    continue;
                }

                GameEntity cardToDraw = FindAvailableCard(playerId);
                
                if (cardToDraw == null)
                {
                    Debug.Log($"[ProcessDrawCardRequestSystem] No cards left in deck for player {playerId}");
                    request.isDestructed = true;
                    continue;
                }

                player.CardsInHand.Add(cardToDraw.Id);
                Debug.Log($"[ProcessDrawCardRequestSystem] Player {playerId} drew card {cardToDraw.Id} (HP={cardToDraw.Hp}, DMG={cardToDraw.Damage})");

                request.isDestructed = true;
            }
        }

        private GameEntity FindAvailableCard(int ownerId)
        {
            foreach (GameEntity card in _deckCards)
            {
                if (card.CardOwner == ownerId && 
                    card.isInHand && 
                    !IsCardInAnyHand(card.Id))
                {
                    return card;
                }
            }
            return null;
        }

        private bool IsCardInAnyHand(int cardId)
        {
            foreach (GameEntity entity in _playersWithHand)
            {
                if (entity.CardsInHand.Contains(cardId))
                    return true;
            }
            return false;
        }
    }
}

