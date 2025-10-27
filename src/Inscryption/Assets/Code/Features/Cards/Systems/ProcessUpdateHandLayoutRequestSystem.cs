using System.Collections.Generic;
using Code.Features.Layout.Services;
using Code.Infrastructure.Level;
using DG.Tweening;
using Entitas;
using UnityEngine;
namespace Code.Features.Cards.Systems
{
    public class ProcessUpdateHandLayoutRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly ILevelProvider _levelProvider;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _requestBuffer = new(1);
        private const float AnimationDuration = 0.2f;
        public ProcessUpdateHandLayoutRequestSystem(GameContext game, ILevelProvider levelProvider)
        {
            _game = game;
            _levelProvider = levelProvider;
            _requests = game.GetGroup(GameMatcher.UpdateHandLayoutRequest);
        }
        public void Execute()
        {
            foreach (GameEntity request in _requests.GetEntities(_requestBuffer))
            {
                int playerId = request.updateHandLayoutRequest.PlayerId;
                GameEntity player = _game.GetEntityWithId(playerId);
                if (player != null && player.hasCardsInHand && player.CardsInHand.Count > 0)
                {
                    UpdateHandLayout(player);
                }
                request.isDestructed = true;
            }
        }
        private void UpdateHandLayout(GameEntity player)
        {
            Transform parent = player.isHero ? _levelProvider.HeroCardParent : _levelProvider.EnemyCardParent;
            var arcLayout = new ArcLayoutParams
            {
                Count = player.CardsInHand.Count,
                Origin = parent.position,
                HorizontalSpacing = 0.3f,
                VerticalCurve = 0.05f,
                DepthSpacing = 0.02f,
                AnglePerCard = 5f
            };
            CardLayoutData[] layoutData = PositionCalculator.CalculateArcLayout(arcLayout);
            for (int i = 0; i < player.CardsInHand.Count; i++)
            {
                int cardId = player.CardsInHand[i];
                GameEntity card = _game.GetEntityWithId(cardId);
                if (card == null || !card.hasTransform || !card.isInHand)
                    continue;
                CardLayoutData layout = layoutData[i];
                card.Transform.DOMove(layout.Position, AnimationDuration).SetEase(Ease.OutQuad);
                if (card.hasVisualTransform && card.VisualTransform != null)
                {
                    card.VisualTransform.localRotation = layout.Rotation;
                    Debug.Log($"[ProcessUpdateHandLayoutRequestSystem] Set local rotation for card {card.Id} to {layout.Rotation.eulerAngles}");
                }
            }
            string playerName = player.isHero ? "Hero" : "Enemy";
            Debug.Log($"[ProcessUpdateHandLayoutRequestSystem] Updated layout for {player.CardsInHand.Count} {playerName} cards");
        }
    }
}
