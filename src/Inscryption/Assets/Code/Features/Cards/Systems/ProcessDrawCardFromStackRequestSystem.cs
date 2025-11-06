using System.Collections.Generic;
using Code.Common;
using Code.Common.Extensions;
using Code.Features.Cards.Components;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class ProcessDrawCardFromStackRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly GameContext _gameContext;
        private readonly List<GameEntity> _buffer = new(5);

        public ProcessDrawCardFromStackRequestSystem(GameContext gameContext)
        {
            _gameContext = gameContext;
            _requests = gameContext.GetGroup(GameMatcher.DrawCardFromStackRequest);
        }

        public void Execute()
        {
            foreach (GameEntity requestEntity in _requests.GetEntities(_buffer))
            {
                ProcessDrawCardFromStackRequest(requestEntity);
            }
        }

        private void ProcessDrawCardFromStackRequest(GameEntity requestEntity)
        {
            DrawCardFromStackRequest request = requestEntity.drawCardFromStackRequest;
            GameEntity stackEntity = _gameContext.GetEntityWithId(request.StackEntityId);
            GameEntity owner = _gameContext.GetEntityWithId(request.OwnerId);

            if (!ValidateRequest(stackEntity, owner))
            {
                requestEntity.Destroy();
                return;
            }

            int cardsToDrawCount = CalculateCardsToDraw(request, stackEntity);
            DrawCardsFromStack(cardsToDrawCount, stackEntity, owner, request);
            requestEntity.Destroy();
        }

        private bool ValidateRequest(GameEntity stackEntity, GameEntity owner)
        {
            if (!ValidateStackEntity(stackEntity))
                return false;

            if (!ValidateOwner(owner))
                return false;

            return true;
        }

        private bool ValidateStackEntity(GameEntity stackEntity)
        {
            if (stackEntity?.hasCardStack != true)
            {
                Debug.LogWarning("[ProcessDrawCardFromStackRequestSystem] Stack entity is invalid or has no CardStack");
                return false;
            }
            return true;
        }

        private bool ValidateOwner(GameEntity owner)
        {
            if (owner?.hasCardsInHand != true)
            {
                Debug.LogWarning("[ProcessDrawCardFromStackRequestSystem] Owner entity is invalid or has no CardsInHand");
                return false;
            }
            return true;
        }

        private int CalculateCardsToDraw(DrawCardFromStackRequest request, GameEntity stackEntity)
        {
            return Mathf.Min(request.CardsToDraw, stackEntity.CardStack.Count);
        }

        private void DrawCardsFromStack(int cardsToDrawCount, GameEntity stackEntity, GameEntity owner, DrawCardFromStackRequest request)
        {
            for (int i = 0; i < cardsToDrawCount; i++)
            {
                DrawCard(i, stackEntity, owner, request);
            }
        }

        private void DrawCard(int index, GameEntity stackEntity, GameEntity owner, DrawCardFromStackRequest request)
        {
            int cardId = stackEntity.CardStack.Pop();
            GameEntity card = _gameContext.GetEntityWithId(cardId);

            if (card == null)
            {
                Debug.LogWarning($"[ProcessDrawCardFromStackRequestSystem] Card with id {cardId} not found");
                return;
            }

            EnsureCardPositionApplied(card);
            PrepareCardForDrawing(card, owner);
            AnimateCardDrawing(card, owner, index, request);
        }

        private void EnsureCardPositionApplied(GameEntity card)
        {
            if (card.hasLocalPosition && card.hasTransform && card.Transform != null)
            {
                card.Transform.localPosition = card.localPosition.Value;
            }
        }

        private void PrepareCardForDrawing(GameEntity card, GameEntity owner)
        {
            card.isStatic = true;
            owner.CardsInHand.Add(card.Id);
            card.ReplaceCardOwner(owner.Id);
        }

        private void AnimateCardDrawing(GameEntity card, GameEntity owner, int index, DrawCardFromStackRequest request)
        {
            float delay = index * request.DelayBetweenCards;

            DOTween.Sequence()
                .AppendInterval(delay)
                .Append(card.Transform.DOMove(request.TargetPosition, request.MoveDuration))
                .OnComplete(() => FinalizeCardDrawing(card, owner, request.Parent));
        }

        private void FinalizeCardDrawing(GameEntity card, GameEntity owner, Transform parent)
        {
            if (card == null || owner == null)
                return;

            UpdateCardHandState(card, parent);
            SetCardSelectionAvailability(card, owner);
            RequestHandLayoutUpdate(owner);
        }

        private void UpdateCardHandState(GameEntity card, Transform parent)
        {
            card.isInHand = true;
            card.SetParent(parent, true);
            Vector3 localPos = card.Transform.localPosition;
            Vector3 localRot = card.Transform.localRotation.eulerAngles;
            card.ReplaceLocalPosition(localPos);
            card.ReplaceLocalRotation(Quaternion.Euler(localRot));
        }

        private void SetCardSelectionAvailability(GameEntity card, GameEntity owner)
        {
            if (owner.isHero)
                card.isSelectionAvailable = true;
        }

        private void RequestHandLayoutUpdate(GameEntity owner)
        {
            CreateEntity.Request()
                .AddUpdateHandLayoutRequest(owner.Id);
        }
    }
}