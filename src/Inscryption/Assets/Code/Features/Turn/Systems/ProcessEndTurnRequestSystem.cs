using System.Collections.Generic;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class ProcessEndTurnRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _attackPhases;
        private readonly List<GameEntity> _buffer = new(4);

        public ProcessEndTurnRequestSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;

            _endTurnRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.EndTurnRequest));

            _attackPhases = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.AttackPhase));
        }

        public void Execute()
        {
            if (_endTurnRequests.count == 0)
                return;

            if (_attackPhases.count > 0)
            {
                foreach (GameEntity request in _endTurnRequests.GetEntities(_buffer))
                {
                    request.isDestructed = true;
                }
                return;
            }

            GameEntity firstRequest = _endTurnRequests.GetEntities(_buffer)[0];
            
            GameEntity activePlayer = GetActivePlayer();

            if (activePlayer == null)
            {
                Debug.LogWarning("[ProcessEndTurnRequestSystem] No active player found!");
                firstRequest.isDestructed = true;
                return;
            }

            Debug.Log($"[ProcessEndTurnRequestSystem] End turn for player {activePlayer.Id}, starting attack phase");

            _game.CreateEntity().isAttackPhase = true;

            foreach (GameEntity request in _endTurnRequests.GetEntities(_buffer))
            {
                request.isDestructed = true;
            }
        }

        private GameEntity GetActivePlayer()
        {
            GameEntity hero = _heroProvider.GetActiveHero();
            if (hero != null)
                return hero;

            GameEntity enemy = _enemyProvider.GetActiveEnemy();
            if (enemy != null)
                return enemy;

            return null;
        }
    }
}

