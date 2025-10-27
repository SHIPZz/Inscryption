using System.Collections.Generic;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class ProcessTurnTransitionSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly List<GameEntity> _requestBuffer = new(8);

        private const float EnemyTurnDelay = 1f;

        public ProcessTurnTransitionSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
            _endTurnRequests = game.GetGroup(GameMatcher.EndTurnRequest);
        }

        public void Execute()
        {
            foreach (GameEntity request in _endTurnRequests.GetEntities(_requestBuffer))
            {
                Debug.Log("[ProcessTurnTransitionSystem] Processing EndTurnRequest");
                ProcessEndTurn();
                request.isDestructed = true;
            }
        }

        private void ProcessEndTurn()
        {
            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            if (hero != null && hero.isHeroTurn)
            {
                Debug.Log("[ProcessTurnTransitionSystem] Hero turn ending - switching to enemy");

                hero.isHeroTurn = false;
                if (enemy != null)
                {
                    enemy.isEnemyTurn = true;
                    enemy.AddPhaseTimer(0f, EnemyTurnDelay);
                    Debug.Log($"[ProcessTurnTransitionSystem] Enemy turn started - waiting {EnemyTurnDelay}s");
                }

                return;
            }

            if (enemy != null && enemy.isEnemyTurn)
            {
                Debug.Log("[ProcessTurnTransitionSystem] Enemy turn ending - switching to hero");

                enemy.isEnemyTurn = false;
                if (hero != null)
                {
                    hero.isHeroTurn = true;
                    hero.ReplaceCardsPlacedThisTurn(0);
                }

                _game.CreateEntity().isSwitchTurnRequest = true;
            }
        }
    }
}
