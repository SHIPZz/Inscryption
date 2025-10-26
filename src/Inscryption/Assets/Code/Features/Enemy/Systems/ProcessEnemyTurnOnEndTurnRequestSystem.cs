using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Features.Enemy.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Enemy.Systems
{
    public class ProcessEnemyTurnOnEndTurnRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _enemySlots;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessEnemyTurnOnEndTurnRequestSystem(GameContext game, IEnemyProvider enemyProvider)
        {
            _game = game;
            _enemyProvider = enemyProvider;

            _endTurnRequests = game.GetGroup(GameMatcher.EndTurnRequest);
            _enemySlots = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.BoardSlot, GameMatcher.EnemyOwner)
                .NoneOf(GameMatcher.Occupied));
        }

        public void Execute()
        {
            foreach (var endTurnRequest in _endTurnRequests.GetEntities(_buffer))
            {
                GameEntity enemy = _enemyProvider.GetEnemy();

                Debug.Log($"@@@: {enemy}");

                if (enemy == null || enemy.CardsInHand.Count == 0)
                {
                    Debug.LogWarning("[ProcessEnemyTurnSystem] Enemy has no cards in hand");
                    return;
                }

                if (_enemySlots.count <= 0)
                {
                    Debug.LogWarning("[ProcessEnemyTurnSystem] No empty slots available for enemy");
                    return;
                }

                GameEntity randomSlot = _enemySlots.AsEnumerable().PickRandom();

                int cardId = enemy.CardsInHand[0];

                _game.CreateEntity()
                    .AddPlaceCardRequest(cardId, randomSlot.Id);

                Debug.Log($"[ProcessEnemyTurnSystem] Enemy placing card {cardId} on slot {randomSlot.Id}");

                endTurnRequest.isDestructed = true;
            }
        }
    }
}
