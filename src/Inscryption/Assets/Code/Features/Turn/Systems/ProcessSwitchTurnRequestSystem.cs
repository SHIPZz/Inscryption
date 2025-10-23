using System.Collections.Generic;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class ProcessSwitchTurnRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly List<GameEntity> _buffer = new(4);

        public ProcessSwitchTurnRequestSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;

            _switchTurnRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwitchTurnRequest));
        }

        public void Execute()
        {
            if (_switchTurnRequests.count == 0)
                return;

            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            if (hero == null || enemy == null)
            {
                Debug.LogWarning("[ProcessSwitchTurnRequestSystem] Hero or Enemy not found!");
                DestroyAllRequests();
                return;
            }

            ProcessTurnSwitch(hero, enemy);
            DestroyAllRequests();
        }

        private void ProcessTurnSwitch(GameEntity hero, GameEntity enemy)
        {
            if (hero.isHeroTurn)
            {
                hero.isHeroTurn = false;
                enemy.isEnemyTurn = true;
                ResetCardsPlaced(hero);
                ResetCardsPlaced(enemy);
                _game.CreateEntity().AddDrawCardRequest(enemy.Id);
                Debug.Log($"[ProcessSwitchTurnRequestSystem] Turn switched to Enemy (ID={enemy.Id})");
            }
            else if (enemy.isEnemyTurn)
            {
                enemy.isEnemyTurn = false;
                hero.isHeroTurn = true;
                ResetCardsPlaced(enemy);
                ResetCardsPlaced(hero);
                _game.CreateEntity().AddDrawCardRequest(hero.Id);
                Debug.Log($"[ProcessSwitchTurnRequestSystem] Turn switched to Hero (ID={hero.Id})");
            }
        }

        private void DestroyAllRequests()
        {
            foreach (GameEntity request in _switchTurnRequests.GetEntities(_buffer))
            {
                request.isDestructed = true;
            }
        }

        private void ResetCardsPlaced(GameEntity player)
        {
            if (player.hasCardsPlacedThisTurn)
                player.ReplaceCardsPlacedThisTurn(0);
        }
    }
}

