using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class ProcessDrawCardFromStackSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _drawRequests;
        private readonly List<GameEntity> _buffer = new(8);

        private const int MaxHandSize = 5;

        public ProcessDrawCardFromStackSystem(GameContext game)
        {
            _game = game;
            _drawRequests = game.GetGroup(GameMatcher.AllOf(GameMatcher.DrawCardFromStackRequest));
        }

        public void Execute()
        {
            foreach (GameEntity request in _drawRequests.GetEntities(_buffer))
            {
                ProcessDrawRequest(request);
                request.isDestructed = true;
            }
        }

        private void ProcessDrawRequest(GameEntity request)
        {
            int stackId = request.drawCardFromStackRequest.StackId;
            int ownerId = request.drawCardFromStackRequest.OwnerId;

            if (!TryGetEntities(stackId, ownerId, out var stack, out var owner))
                return;

            if (!ValidateDrawConditions(stack, owner, stackId, ownerId))
                return;

            int cardId = stack.cardStack.Value.Pop();
            GameEntity card = _game.GetEntityWithId(cardId);

            if (!ValidateCard(card, cardId))
                return;

            TransferCardToOwner(card, owner, cardId, ownerId);
            LogSuccessfulDraw(ownerId, cardId, stackId);
        }

        private bool TryGetEntities(int stackId, int ownerId, out GameEntity stack, out GameEntity owner)
        {
            stack = _game.GetEntityWithId(stackId);
            owner = _game.GetEntityWithId(ownerId);

            if (stack == null)
            {
                Debug.LogWarning($"[ProcessDrawCardFromStackSystem] Stack {stackId} not found!");
                return false;
            }

            if (owner == null)
            {
                Debug.LogWarning($"[ProcessDrawCardFromStackSystem] Owner {ownerId} not found!");
                return false;
            }

            return true;
        }

        private bool ValidateDrawConditions(GameEntity stack, GameEntity owner, int stackId, int ownerId)
        {
            if (!stack.hasCardStack)
            {
                Debug.LogWarning($"[ProcessDrawCardFromStackSystem] Stack {stackId} has no CardStack component!");
                return false;
            }

            if (!owner.hasCardsInHand)
            {
                Debug.LogWarning($"[ProcessDrawCardFromStackSystem] Owner {ownerId} has no CardsInHand component!");
                return false;
            }

            if (IsHandFull(owner, ownerId))
                return false;

            if (IsStackEmpty(stack, stackId))
                return false;

            return true;
        }

        private bool IsHandFull(GameEntity owner, int ownerId)
        {
            if (owner.cardsInHand.Value.Count >= MaxHandSize)
            {
                Debug.Log($"[ProcessDrawCardFromStackSystem] Owner {ownerId} hand is full ({MaxHandSize} cards), skip draw");
                return true;
            }
            return false;
        }

        private bool IsStackEmpty(GameEntity stack, int stackId)
        {
            if (stack.cardStack.Value.Count == 0)
            {
                Debug.Log($"[ProcessDrawCardFromStackSystem] Stack {stackId} is empty");
                return true;
            }
            return false;
        }

        private bool ValidateCard(GameEntity card, int cardId)
        {
            if (card == null)
            {
                Debug.LogWarning($"[ProcessDrawCardFromStackSystem] Card {cardId} not found!");
                return false;
            }
            return true;
        }

        private void TransferCardToOwner(GameEntity card, GameEntity owner, int cardId, int ownerId)
        {
            card.ReplaceCardOwner(ownerId);
            card.isInHand = true;
            owner.cardsInHand.Value.Add(cardId);
        }

        private void LogSuccessfulDraw(int ownerId, int cardId, int stackId)
        {
            Debug.Log($"[ProcessDrawCardFromStackSystem] Owner {ownerId} drew card {cardId} from stack {stackId}");
        }
    }
}
