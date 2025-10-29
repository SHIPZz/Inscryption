using Code.Common;
using Code.Common.Extensions;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class StartTurnSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly GameConfig _gameConfig;

        public StartTurnSystem(GameContext game, IConfigService configService)
        {
            _game = game;
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _switchTurnRequests = game.GetGroup(GameMatcher.AllOf(GameMatcher.SwitchTurnRequest, GameMatcher.ProcessingAvailable));
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _switchTurnRequests)
            {
                HandleTurnStart();
            }
        }

        private void HandleTurnStart()
        {
            foreach (var hero in _heroes)
            {
                if (hero.isHeroTurn)
                {
                    ProcessEntityTurn(hero);
                    return;
                }
            }

            foreach (var enemy in _enemies)
            {
                if (enemy.isEnemyTurn)
                {
                    ProcessEntityTurn(enemy);
                }
            }
        }

        private void ProcessEntityTurn(GameEntity entity)
        {
            if (entity.hasCardsPlacedThisTurn)
                entity.ReplaceCardsPlacedThisTurn(0);

            var maxHandSize = _gameConfig.GameBalance.MaxHandSize;

            if (entity.CardsInHand.Count < maxHandSize)
                CreateEntity
                    .Request()
                    .AddDrawCardRequest(entity.Id);
        }
    }
}
