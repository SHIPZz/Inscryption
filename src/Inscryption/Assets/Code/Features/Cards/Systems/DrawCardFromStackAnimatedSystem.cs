using Code.Common.Services;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class DrawCardFromStackAnimatedSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly GameContext _gameContext;
        private readonly ICameraProvider _cameraProvider;

        public DrawCardFromStackAnimatedSystem(GameContext gameContext, ICameraProvider cameraProvider)
        {
            _gameContext = gameContext;
            _cameraProvider = cameraProvider;
            _requests = gameContext.GetGroup(GameMatcher.DrawCardFromStackRequest);
        }

        public void Execute()
        {
            foreach (GameEntity requestEntity in _requests.GetEntities())
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

                    DOTween.Sequence()
                        .AppendInterval(i * request.DelayBetweenCards)
                        .Append(cardEntity.View.transform.DOMove(request.TargetPositions[i], request.MoveDuration))
                        .Append(cardEntity.View.transform.DORotateQuaternion(_cameraProvider.MainCamera.transform.rotation, 0.5f))
                        .OnComplete(() =>
                        {
                            cardEntity.isSelectionAvailable = true;
                            cardEntity.ReplaceCardOwner(request.OwnerId);
                            cardEntity.ReplaceParent(request.Parent);
                        })
                        ;
                }
                
                requestEntity.Destroy();
            }
        }
    }
}
