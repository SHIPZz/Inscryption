using System.Collections.Generic;
using Code.Common.Time;
using Code.Features.Cooldowns.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    //TODO REFACTOR THIS
    public class ProcessTurnTransitionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly GameConfig _gameConfig;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ITimerService _timerService;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessTurnTransitionSystem(
            GameContext game, 
            IConfigService configService,
            IGameStateMachine gameStateMachine,
            ITimerService timerService)
        {
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _switchTurnRequests = game.GetGroup(GameMatcher.SwitchTurnRequest);
            _gameConfig = configService.GetConfig<GameConfig>();
            _gameStateMachine = gameStateMachine;
            _timerService = timerService;
        }

        public void Execute()
        {
            foreach (GameEntity request in _switchTurnRequests.GetEntities(_buffer))
            {
                ProcessTurnSwitch();
                request.Destroy();
            }
        }

        private void ProcessTurnSwitch()
        {
            GameEntity currentHero = FindHeroInTurn();
            if (currentHero != null)
            {
                SwitchFromHeroToEnemy(currentHero);
                return;
            }

            GameEntity currentEnemy = FindEnemyInTurn();
            if (currentEnemy != null)
            {
                SwitchFromEnemyToHero(currentEnemy);
            }
        }

        private GameEntity FindHeroInTurn()
        {
            foreach (var hero in _heroes)
            {
                if (hero.isHeroTurn)
                    return hero;
            }
            return null;
        }

        private GameEntity FindEnemyInTurn()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.isEnemyTurn)
                    return enemy;
            }
            return null;
        }

        private void SwitchFromHeroToEnemy(GameEntity hero)
        {
            Debug.Log("[ProcessTurnTransitionSystem] Switching from Hero to Enemy");
            ClearHeroTurn(hero);
            StartEnemyTurn();
        }

        private void SwitchFromEnemyToHero(GameEntity enemy)
        {
            Debug.Log("[ProcessTurnTransitionSystem] Switching from Enemy to Hero");
            ClearEnemyTurn(enemy);
            StartHeroTurn();
        }

        private void ClearHeroTurn(GameEntity hero)
        {
            hero.isHeroTurn = false;
        }

        private void ClearEnemyTurn(GameEntity enemy)
        {
            enemy.isEnemyTurn = false;
        }

        private void StartHeroTurn()
        {
            foreach (var hero in _heroes)
            {
                HandleHeroTurn(hero);
            }
        }

        private void StartEnemyTurn()
        {
            foreach (var enemy in _enemies)
            {
                HandleEnemyTurn(enemy);
            }
        }

        private void HandleHeroTurn(GameEntity hero)
        {
            hero.isHeroTurn = true;
            hero.ReplaceCardsPlacedThisTurn(0);
            TransitionToHeroTurn(hero);
        }

        private void TransitionToHeroTurn(GameEntity hero)
        {
            float delay = _gameConfig.AnimationTiming.EnemyTurnDelay + _gameConfig.AnimationTiming.PostAttackDelay;
            _timerService.Schedule(delay, () =>
            {
                _gameStateMachine.EnterAsync<States.HeroTurnState>().Forget();
            });
        }

        private void HandleEnemyTurn(GameEntity enemy)
        {
            enemy.isEnemyTurn = true;
            enemy.ReplaceCardsPlacedThisTurn(0);
            ApplyEnemyTurnCooldown(enemy);
            TransitionToEnemyTurn(enemy);
        }

        private void ApplyEnemyTurnCooldown(GameEntity enemy)
        {
            float enemyTurnDelay = _gameConfig.AnimationTiming.EnemyTurnDelay;
            enemy.PutOnCooldown(enemyTurnDelay);
            Debug.Log($"[ProcessTurnTransitionSystem] Enemy turn started - waiting {enemyTurnDelay}s");
        }

        private void TransitionToEnemyTurn(GameEntity enemy)
        {
            float delay = _gameConfig.AnimationTiming.EnemyTurnDelay;
            _timerService.Schedule(delay, () =>
            {
                _gameStateMachine.EnterAsync<States.EnemyTurnState>().Forget();
            });
        }
    }
}
