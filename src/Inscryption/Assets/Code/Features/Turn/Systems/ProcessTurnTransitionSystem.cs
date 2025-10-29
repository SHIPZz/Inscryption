using Code.Features.Cooldowns.Extensions;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class ProcessTurnTransitionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly GameConfig _gameConfig;

        public ProcessTurnTransitionSystem(GameContext game, IConfigService configService)
        {
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _switchTurnRequests = game.GetGroup(GameMatcher.AllOf(GameMatcher.SwitchTurnRequest, GameMatcher.ProcessingAvailable));
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _switchTurnRequests)
            {
                Debug.Log("[ProcessTurnTransitionSystem] Processing SwitchTurnRequest");
                ProcessTurnSwitch();
            }
        }

        private void ProcessTurnSwitch()
        {
            foreach (var hero in _heroes)
            {
                if (hero.isHeroTurn)
                {
                    Debug.Log("[ProcessTurnTransitionSystem] Switching from Hero to Enemy");
                    hero.isHeroTurn = false;

                    foreach (var enemy in _enemies)
                    {
                        HandleEnemyTurn(enemy);
                    }
                    return;
                }
            }

            foreach (var enemy in _enemies)
            {
                if (enemy.isEnemyTurn)
                {
                    Debug.Log("[ProcessTurnTransitionSystem] Switching from Enemy to Hero");
                    enemy.isEnemyTurn = false;

                    foreach (var hero in _heroes)
                    {
                        HandleHeroTurn(hero);
                    }
                }
            }
        }

        private void HandleHeroTurn(GameEntity hero)
        {
            hero.isHeroTurn = true;
            hero.ReplaceCardsPlacedThisTurn(0);
        }

        private void HandleEnemyTurn(GameEntity enemy)
        {
            var enemyTurnDelay = _gameConfig.AnimationTiming.EnemyTurnDelay;
            enemy.isEnemyTurn = true;
            enemy.ReplaceCardsPlacedThisTurn(0);
            enemy.PutOnCooldown(enemyTurnDelay);
            Debug.Log($"[ProcessTurnTransitionSystem] Enemy turn started - waiting {enemyTurnDelay}s");
        }
    }
}
