using System.Collections.Generic;
using Code.Features.Layout.Services;
using Code.Infrastructure.Level;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class ProcessDrawCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly ILevelProvider _levelProvider;
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _stacks;
        private readonly List<GameEntity> _requestBuffer = new(8);
        private readonly List<GameEntity> _stackBuffer = new(1);
        private const float DelayBetweenCards = 0.5f;
        private const float MoveDuration = 0.25f;

        public ProcessDrawCardRequestSystem(GameContext game, ILevelProvider levelProvider)
        {
            _game = game;
            _levelProvider = levelProvider;
            _requests = game.GetGroup(GameMatcher.DrawCardRequest);
            _stacks = game.GetGroup(GameMatcher.CardStack);
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

            _game.CreateEntity().AddDrawCardFromStackRequest(
                stack.Id,
                1,
                player.Id,
                new[] { targetPosition },
                DelayBetweenCards,
                MoveDuration,
                parent
            );
        }

        private CardLayoutData[] CalculateHandLayout(GameEntity player, Transform parent)
        {
            var arcLayout = new ArcLayoutParams
            {
                Count = player.CardsInHand.Count + 1,
                Origin = parent.position,
                HorizontalSpacing = 0.3f,
                VerticalCurve = 0.05f,
                DepthSpacing = 0.02f,
                AnglePerCard = 5f
            };
            return PositionCalculator.CalculateArcLayout(arcLayout);
        }
    }
}