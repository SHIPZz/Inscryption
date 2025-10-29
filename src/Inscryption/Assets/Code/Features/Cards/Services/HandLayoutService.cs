using Code.Features.Layout.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Level;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Features.Cards.Services
{
    public class HandLayoutService : IHandLayoutService
    {
        private readonly ILevelProvider _levelProvider;
        private readonly GameConfig _gameConfig;

        public HandLayoutService(ILevelProvider levelProvider, IConfigService configService)
        {
            _levelProvider = levelProvider;
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public CardLayoutData[] CalculateLayout(GameEntity player, int additionalCards = 0)
        {
            Transform parent = GetCardParent(player);
            var handLayout = _gameConfig.HandLayout;

            var arcLayout = new ArcLayoutParams
            {
                Count = player.CardsInHand.Count + additionalCards,
                Origin = parent.position,
                HorizontalSpacing = handLayout.HorizontalSpacing,
                VerticalCurve = handLayout.VerticalCurve,
                DepthSpacing = handLayout.DepthSpacing,
                AnglePerCard = handLayout.AnglePerCard
            };

            return PositionCalculator.CalculateArcLayout(arcLayout);
        }

        public Transform GetCardParent(GameEntity player)
        {
            return player.isHero
                ? _levelProvider.HeroCardParent
                : _levelProvider.EnemyCardParent;
        }

        public Vector3 GetLastCardPosition(GameEntity player)
        {
            var layout = CalculateLayout(player, additionalCards: 1);
            return layout[^1].Position;
        }
    }
}
