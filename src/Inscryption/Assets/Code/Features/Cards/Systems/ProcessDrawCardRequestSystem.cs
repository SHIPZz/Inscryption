using System.Collections.Generic;
using Code.Features.Layout.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Level;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    //todo refactor this:
    public class ProcessDrawCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly ILevelProvider _levelProvider;
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _stacks;
        private readonly List<GameEntity> _requestBuffer = new(8);
        private readonly GameConfig _gameConfig;

        public ProcessDrawCardRequestSystem(GameContext game, ILevelProvider levelProvider, IConfigService configService)
        {
            _game = game;
            _levelProvider = levelProvider;
            _requests = game.GetGroup(GameMatcher.DrawCardRequest);
            _stacks = game.GetGroup(GameMatcher.CardStack);
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests.GetEntities(_requestBuffer))
            {
                ProcessDrawCardRequest(request);
            }
        }

        private void ProcessDrawCardRequest(GameEntity request)
        {
            var player = _game.GetEntityWithId(request.drawCardRequest.PlayerId);
            if (player == null)
            {
                request.isDestructed = true;
                return;
            }

            GameEntity stack = _stacks.GetEntities()[0];
            
            if (stack == null || stack.CardStack.Count <= 0)
            {
                request.isDestructed = true;
                return;
            }

            CreateDrawCardFromStackRequest(player, stack);
            request.isDestructed = true;
        }

        private void CreateDrawCardFromStackRequest(GameEntity player, GameEntity stack)
        {
            var parent = player.isHero ? _levelProvider.HeroCardParent : _levelProvider.EnemyCardParent;
            var layoutData = CalculateHandLayout(player, parent);
            var targetPosition = layoutData[^1].Position;
            var animTiming = _gameConfig.AnimationTiming;

            _game.CreateEntity().AddDrawCardFromStackRequest(
                stack.Id,
                1,
                player.Id,
                new[] { targetPosition },
                animTiming.DelayBetweenCards,
                animTiming.CardMoveDuration,
                parent
            );
        }

        private CardLayoutData[] CalculateHandLayout(GameEntity player, Transform parent)
        {
            var handLayout = _gameConfig.HandLayout;
            var arcLayout = new ArcLayoutParams
            {
                Count = player.CardsInHand.Count + 1,
                Origin = parent.position,
                HorizontalSpacing = handLayout.HorizontalSpacing,
                VerticalCurve = handLayout.VerticalCurve,
                DepthSpacing = handLayout.DepthSpacing,
                AnglePerCard = handLayout.AnglePerCard
            };
            return PositionCalculator.CalculateArcLayout(arcLayout);
        }
    }
}