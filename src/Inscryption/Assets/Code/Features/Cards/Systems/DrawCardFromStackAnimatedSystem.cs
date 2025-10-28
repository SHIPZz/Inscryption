using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    //todo refactor this:
    public class DrawCardFromStackAnimatedSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly GameContext _gameContext;
        private readonly List<GameEntity> _buffer = new(5);

        public DrawCardFromStackAnimatedSystem(GameContext gameContext)
        {
            _gameContext = gameContext;
            _requests = gameContext.GetGroup(GameMatcher.DrawCardFromStackRequest);
        }

        public void Execute()
        {
            foreach (GameEntity requestEntity in _requests.GetEntities(_buffer))
            {
                DrawCardFromStackRequest request = requestEntity.drawCardFromStackRequest;
                GameEntity stackEntity = _gameContext.GetEntityWithId(request.StackEntityId);
                for (int i = 0; i < request.CardsToDraw; i++)
                {
                    if (i >= request.TargetPositions.Count)
                    {
                        Debug.LogWarning("Not enough target positions for cards to draw.");
                        break;
                    }

                    int cardId = stackEntity.CardStack.Pop();
                    var cardEntity = _gameContext.GetEntityWithId(cardId);
                    cardEntity.isStatic = true;
                    GameEntity cardOwner = _gameContext.GetEntityWithId(request.OwnerId);
                    cardOwner.CardsInHand.Add(cardEntity.Id);
                    cardEntity.ReplaceCardOwner(request.OwnerId);
                    DOTween.Sequence()
                        .AppendInterval(i * request.DelayBetweenCards)
                        .Append(cardEntity.Transform.DOMove(request.TargetPositions[i], request.MoveDuration))
                        .OnComplete(() =>
                        {
                            if (cardOwner != null && cardOwner.isHero)
                                cardEntity.isSelectionAvailable = true;
                            cardEntity.isInHand = true;
                            cardEntity.ReplaceParent(request.Parent);
                            cardEntity.ReplaceLocalPosition(cardEntity.Transform.localPosition);
                            cardEntity.ReplaceLocalRotation(cardEntity.Transform.localRotation);
                            _gameContext.CreateEntity().AddUpdateHandLayoutRequest(cardOwner.Id);
                        })
                        ;
                }

                requestEntity.Destroy();
            }
        }
    }
}