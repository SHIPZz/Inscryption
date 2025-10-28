using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class StartTurnSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly List<GameEntity> _requestBuffer = new(1);
        private readonly GameConfig _gameConfig;

        public StartTurnSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider, IConfigService configService)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
            _switchTurnRequests = game.GetGroup(GameMatcher.AllOf(GameMatcher.SwitchTurnRequest, GameMatcher.ProcessingAvailable));
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _switchTurnRequests.GetEntities(_requestBuffer))
            {
                HandleTurnStart();
                
                request.isDestructed = true;
            }
        }

        private void HandleTurnStart()
        {
            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            if (hero != null && hero.isHeroTurn)
            {
                ProcessEntityTurn(hero);
                return;
            }

            if (enemy != null && enemy.isEnemyTurn)
            {
                ProcessEntityTurn(enemy);
            }
        }

        private void ProcessEntityTurn(GameEntity entity)
        {
            if (entity.hasCardsPlacedThisTurn)
                entity.ReplaceCardsPlacedThisTurn(0);

            var maxHandSize = _gameConfig.GameBalance.MaxHandSize;

            if (entity.CardsInHand.Count < maxHandSize)
                _game.CreateEntity().AddDrawCardRequest(entity.Id)
                    .With(x => x.isRequest = true);
        }
    }
}
