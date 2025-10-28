using System.Collections.Generic;
using Code.Features.Layout.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Level;
using Code.Infrastructure.Services;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    //todo refactor this
    public class ProcessUpdateHandLayoutRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly ILevelProvider _levelProvider;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _requestBuffer = new(1);
        private readonly GameConfig _gameConfig;

        public ProcessUpdateHandLayoutRequestSystem(GameContext game, ILevelProvider levelProvider,
            IConfigService configService)
        {
            _game = game;
            _levelProvider = levelProvider;
            _requests = game.GetGroup(GameMatcher.UpdateHandLayoutRequest);
            _gameConfig = configService.GetConfig<GameConfig>();
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
            var handLayout = _gameConfig.HandLayout;
            var arcLayout = new ArcLayoutParams
            {
                Count = player.CardsInHand.Count,
                Origin = parent.position,
                HorizontalSpacing = handLayout.HorizontalSpacing,
                VerticalCurve = handLayout.VerticalCurve,
                DepthSpacing = handLayout.DepthSpacing,
                AnglePerCard = handLayout.AnglePerCard
            };
            CardLayoutData[] layoutData = PositionCalculator.CalculateArcLayout(arcLayout);
            var animDuration = _gameConfig.AnimationTiming.LayoutUpdateDuration;
            for (int i = 0; i < player.CardsInHand.Count; i++)
            {
                int cardId = player.CardsInHand[i];
                GameEntity card = _game.GetEntityWithId(cardId);
                if (card == null || !card.hasTransform || !card.isInHand)
                    continue;
                CardLayoutData layout = layoutData[i];
                card.Transform.DOMove(layout.Position, animDuration).SetEase(Ease.OutQuad);
                if (card.hasVisualTransform && card.VisualTransform != null)
                {
                    card.VisualTransform.localRotation = layout.Rotation;
                    Debug.Log(
                        $"[ProcessUpdateHandLayoutRequestSystem] Set local rotation for card {card.Id} to {layout.Rotation.eulerAngles}");
                }
            }

            string playerName = player.isHero ? "Hero" : "Enemy";
            Debug.Log(
                $"[ProcessUpdateHandLayoutRequestSystem] Updated layout for {player.CardsInHand.Count} {playerName} cards");
        }
    }
}