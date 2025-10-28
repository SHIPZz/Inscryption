using System.Collections.Generic;
using Code.Common.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Features.Enemy.Systems
{
    //todo refactor this: _enemyTurnCooldowns, enemyprovider
    public class ProcessEnemyTurnOnCooldownSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _enemyTurnCooldowns;
        private readonly IGroup<GameEntity> _enemySlots;
        private readonly List<GameEntity> _buffer = new(2);

        public ProcessEnemyTurnOnCooldownSystem(GameContext game)
        {
            _game = game;
            _enemyTurnCooldowns = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.EnemyTurn, 
                GameMatcher.CooldownUp,
                GameMatcher.Enemy));
            
            _enemySlots = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.BoardSlot, GameMatcher.EnemyOwner)
                .NoneOf(GameMatcher.Occupied));
        }

        public void Execute()
        {
            foreach (var enemy in _enemyTurnCooldowns.GetEntities(_buffer))
            {
                ExecuteEnemyTurn(enemy);

                enemy.isCooldownUp = false;
            }
        }

        private void ExecuteEnemyTurn(GameEntity enemy)
        {
            Debug.Log("[ProcessEnemyTurnSystem] Enemy turn timer expired");

            if (enemy == null)
            {
                Debug.LogWarning("[ProcessEnemyTurnSystem] Enemy not found");
                return;
            }

            if (CanPlaceCard(enemy))
                PlaceEnemyCard(enemy);

            _game.CreateEntity().isEndTurnRequest = true;
        }

        private bool CanPlaceCard(GameEntity enemy) =>
            enemy.CardsInHand.Count > 0 && _enemySlots.count > 0;

        private void PlaceEnemyCard(GameEntity enemy)
        {
            var randomSlot = _enemySlots.AsEnumerable().PickRandom();
            var cardId = enemy.CardsInHand[0];
            _game.CreateEntity().AddPlaceCardRequest(cardId, randomSlot.Id);
            Debug.Log($"[ProcessEnemyTurnSystem] Enemy placing card {cardId} on slot {randomSlot.Id}");
        }
    }
}