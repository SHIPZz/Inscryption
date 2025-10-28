using System.Collections.Generic;
using Code.Features.Cooldowns.Extensions;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class ProcessTurnTransitionSystem : IExecuteSystem
    {
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly List<GameEntity> _requestBuffer = new(8);
        private readonly GameConfig _gameConfig;

        public ProcessTurnTransitionSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider, IConfigService configService)
        {
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
            _switchTurnRequests = game.GetGroup(GameMatcher.SwitchTurnRequest);
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _switchTurnRequests.GetEntities(_requestBuffer))
            {
                Debug.Log("[ProcessTurnTransitionSystem] Processing SwitchTurnRequest");
                ProcessTurnSwitch();
                request.isDestructed = true;
            }
        }

        private void ProcessTurnSwitch()
        {
            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            if (hero is { isHeroTurn: true })
            {
                Debug.Log("[ProcessTurnTransitionSystem] Switching from Hero to Enemy");

                hero.isHeroTurn = false;
                
                HandleEnemyTurn(hero, enemy);

                return;
            }

            if (enemy is { isEnemyTurn: true }) 
                HandleHeroTurn(enemy, hero);
        }

        private static void HandleHeroTurn(GameEntity enemy, GameEntity hero)
        {
            Debug.Log("[ProcessTurnTransitionSystem] Switching from Enemy to Hero");

            enemy.isEnemyTurn = false;
         
            if (hero != null)
            {
                hero.isHeroTurn = true;
                hero.ReplaceCardsPlacedThisTurn(0);
            }
        }

        private void HandleEnemyTurn(GameEntity hero, GameEntity enemy)
        {
            hero.isHeroTurn = false;
            
            if (enemy != null)
            {
                var enemyTurnDelay = _gameConfig.AnimationTiming.EnemyTurnDelay;
                enemy.isEnemyTurn = true;
                enemy.ReplaceCardsPlacedThisTurn(0);
                enemy.PutOnCooldown(enemyTurnDelay);
                Debug.Log($"[ProcessTurnTransitionSystem] Enemy turn started - waiting {enemyTurnDelay}s");
            }
        }
    }
}
