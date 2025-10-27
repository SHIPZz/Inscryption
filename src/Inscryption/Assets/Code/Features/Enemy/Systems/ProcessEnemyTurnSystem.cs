using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Common.Time;
using Code.Features.Enemy.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Enemy.Systems
{
    public class ProcessEnemyTurnSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly ITimeService _timeService;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _enemyTurnTimers;
        private readonly IGroup<GameEntity> _enemySlots;
        private readonly List<GameEntity> _timerBuffer = new(2);

        public ProcessEnemyTurnSystem(GameContext game, ITimeService timeService, IEnemyProvider enemyProvider)
        {
            _game = game;
            _timeService = timeService;
            _enemyProvider = enemyProvider;
            _enemyTurnTimers = game.GetGroup(GameMatcher.AllOf(GameMatcher.EnemyTurn, GameMatcher.PhaseTimer));
            _enemySlots = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.BoardSlot, GameMatcher.EnemyOwner)
                .NoneOf(GameMatcher.Occupied));
        }

        public void Execute()
        {
            foreach (var timerEntity in _enemyTurnTimers.GetEntities(_timerBuffer))
            {
                UpdateTimer(timerEntity);
                if (IsTimerExpired(timerEntity))
                {
                    ExecuteEnemyTurn(timerEntity);
                }
            }
        }

        private void UpdateTimer(GameEntity timerEntity)
        {
            var timer = timerEntity.phaseTimer;
            timer.ElapsedTime += _timeService.DeltaTime;
            timerEntity.ReplacePhaseTimer(timer.ElapsedTime, timer.TargetDuration);
        }

        private bool IsTimerExpired(GameEntity timerEntity) =>
            timerEntity.phaseTimer.ElapsedTime >= timerEntity.phaseTimer.TargetDuration;

        private void ExecuteEnemyTurn(GameEntity timerEntity)
        {
            Debug.Log("[ProcessEnemyTurnSystem] Enemy turn timer expired - placing card");

            var enemy = _enemyProvider.GetEnemy();
            if (enemy == null)
            {
                Debug.LogWarning("[ProcessEnemyTurnSystem] Enemy not found");
                timerEntity.RemovePhaseTimer();
                return;
            }

            if (CanPlaceCard(enemy))
            {
                PlaceEnemyCard(enemy);
            }
            else
            {
                Debug.Log("[ProcessEnemyTurnSystem] Enemy has no cards or no slots");
            }

            EndEnemyTurn(timerEntity);
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

        private void EndEnemyTurn(GameEntity timerEntity)
        {
            _game.CreateEntity().isEndTurnRequest = true;
            timerEntity.RemovePhaseTimer();
            Debug.Log("[ProcessEnemyTurnSystem] Enemy EndTurnRequest created");
        }
    }
}
