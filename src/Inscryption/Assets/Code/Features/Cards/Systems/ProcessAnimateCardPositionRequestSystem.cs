using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class ProcessAnimateCardPositionRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _requests;
        private readonly GameConfig _gameConfig;

        public ProcessAnimateCardPositionRequestSystem(GameContext game, IConfigService configService)
        {
            _game = game;
            _requests = game.GetGroup(GameMatcher.AllOf(GameMatcher.AnimateCardPositionRequest, GameMatcher.ProcessingAvailable));
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests)
            {
                int cardId = request.animateCardPositionRequest.CardId;
                GameEntity card = _game.GetEntityWithId(cardId);

                if (IsValidCard(card))
                {
                    AnimateCard(card, request.animateCardPositionRequest.Position, request.animateCardPositionRequest.Rotation);
                }
            }
        }

        private bool IsValidCard(GameEntity card)
        {
            return card != null && card.hasTransform;
        }

        private void AnimateCard(GameEntity card, Vector3 position, Quaternion rotation)
        {
            float animDuration = _gameConfig.AnimationTiming.LayoutUpdateDuration;

            card.Transform.DOMove(position, animDuration).SetEase(Ease.OutQuad);

            if (card.hasVisualTransform && card.VisualTransform != null)
            {
                card.VisualTransform.localRotation = rotation;
                Debug.Log($"[ProcessAnimateCardPositionRequestSystem] Set local rotation for card {card.Id} to {rotation.eulerAngles}");
            }
        }
    }
}
