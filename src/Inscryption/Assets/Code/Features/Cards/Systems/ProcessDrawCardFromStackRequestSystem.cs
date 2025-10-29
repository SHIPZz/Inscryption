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
            _requests = gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.DrawCardFromStackRequest,
                GameMatcher.ProcessingAvailable));
        }

        public void Execute()
        {
            foreach (GameEntity requestEntity in _requests.GetEntities(_buffer))
            {
                DrawCardFromStackRequest request = requestEntity.drawCardFromStackRequest;
                GameEntity stackEntity = _gameContext.GetEntityWithId(request.StackEntityId);
                GameEntity owner = _gameContext.GetEntityWithId(request.OwnerId);

                if (!ValidateRequest(stackEntity, owner))
                {
                    continue;
                }

                int cardsToDrawCount = Mathf.Min(request.CardsToDraw, stackEntity.CardStack.Count);

                for (int i = 0; i < cardsToDrawCount; i++)
                {
                    DrawCard(i, stackEntity, owner, request);
                }
            }
        }

        private bool ValidateRequest(GameEntity stackEntity, GameEntity owner)
        {
            if (stackEntity?.hasCardStack != true)
            {
                Debug.LogWarning("[DrawCardFromStackAnimatedSystem] Stack entity is invalid or has no CardStack");
                return false;
            }

            if (owner?.hasCardsInHand != true)
            {
                Debug.LogWarning("[DrawCardFromStackAnimatedSystem] Owner entity is invalid or has no CardsInHand");
                return false;
            }

            return true;
        }

        private void DrawCard(int index, GameEntity stackEntity, GameEntity owner, DrawCardFromStackRequest request)
        {
            int cardId = stackEntity.CardStack.Pop();
            GameEntity card = _gameContext.GetEntityWithId(cardId);

            if (card == null)
            {
                Debug.LogWarning($"[DrawCardFromStackAnimatedSystem] Card with id {cardId} not found");
                return;
            }

            PrepareCard(card, owner);
            AnimateCard(card, owner, index, request);
        }

        private void PrepareCard(GameEntity card, GameEntity owner)
        {
            card.isStatic = true;
            owner.CardsInHand.Add(card.Id);
            card.ReplaceCardOwner(owner.Id);
        }

        private void AnimateCard(GameEntity card, GameEntity owner, int index, DrawCardFromStackRequest request)
        {
            float delay = index * request.DelayBetweenCards;

            DOTween.Sequence()
                .AppendInterval(delay)
                .Append(card.Transform.DOMove(request.TargetPosition, request.MoveDuration))
                .OnComplete(() => FinalizeCard(card, owner, request.Parent));
        }

        private void FinalizeCard(GameEntity card, GameEntity owner, Transform parent)
        {
            if (card == null || owner == null)
                return;

            card.isInHand = true;
            card.ReplaceParent(parent);
            card.ReplaceLocalPosition(card.Transform.localPosition);
            card.ReplaceLocalRotation(card.Transform.localRotation);

            if (owner.isHero)
                card.isSelectionAvailable = true;

            CreateEntity.Request()
                .AddUpdateHandLayoutRequest(owner.Id);
        }
    }
}