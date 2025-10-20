using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class ProcessEndTurnRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _attackPhases;
        private readonly List<GameEntity> _buffer = new(4);

        public ProcessEndTurnRequestSystem(GameContext game)
        {
            _game = game;

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
            foreach (GameEntity entity in _game.GetEntities(GameMatcher.Hero))
            {
                if (entity.isHeroTurn)
                    return entity;
            }

            foreach (GameEntity entity in _game.GetEntities(GameMatcher.Enemy))
            {
                if (entity.isEnemyTurn)
                    return entity;
            }

            return null;
        }
    }
}

