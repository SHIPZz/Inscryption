using System.Collections.Generic;
using Code.Common;
using Code.Features.Cards.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;

namespace Code.Features.Cards.Systems
{
    public class ProcessDrawCardRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHandLayoutService _handLayoutService;
        private readonly IGroup<GameEntity> _requests;
        private readonly IGroup<GameEntity> _stacks;
        private readonly List<GameEntity> _requestBuffer = new(8);
        private readonly GameConfig _gameConfig;

        public ProcessDrawCardRequestSystem(GameContext game, IHandLayoutService handLayoutService, IConfigService configService)
        {
            _game = game;
            _handLayoutService = handLayoutService;
            _requests = game.GetGroup(GameMatcher.AllOf(GameMatcher.DrawCardRequest, GameMatcher.ProcessingAvailable));
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
            var parent = _handLayoutService.GetCardParent(player);
            var targetPosition = _handLayoutService.GetLastCardPosition(player);
            var animTiming = _gameConfig.AnimationTiming;

            CreateEntity.Request()
                .AddDrawCardFromStackRequest(
                stack.Id,
                newCardsToDraw: 1,
                player.Id,
                targetPosition,
                animTiming.DelayBetweenCards,
                animTiming.CardMoveDuration,
                parent
            );
        }
    }
}