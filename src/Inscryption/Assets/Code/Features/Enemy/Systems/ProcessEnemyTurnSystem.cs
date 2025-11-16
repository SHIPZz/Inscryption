using System.Collections.Generic;
using Code.Common;
using Code.Common.Extensions;
using Code.Common.Time;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Enemy.Systems
{
  public class ProcessEnemyTurnSystem : IExecuteSystem
  {
    private readonly GameContext _game;
    private readonly ITimerService _timerService;
    private readonly IGroup<GameEntity> _enemies;
    private readonly IGroup<GameEntity> _enemySlots;
    private readonly GameConfig _gameConfig;
    private readonly List<GameEntity> _buffer = new(2);

    private bool _enemyTurnScheduled;

    public ProcessEnemyTurnSystem(GameContext game, ITimerService timerService, IConfigService configService)
    {
      _game = game;
      _timerService = timerService;
      _gameConfig = configService.GetConfig<GameConfig>();

      _enemies = game.GetGroup(GameMatcher.AllOf(GameMatcher.Enemy, GameMatcher.EnemyTurn));
      _enemySlots = game.GetGroup(GameMatcher.AllOf(GameMatcher.BoardSlot, GameMatcher.EnemyOwner).NoneOf(GameMatcher.Occupied));
    }

    public void Execute()
    {
      if (_enemies.count == 0)
        return;

      if (_enemyTurnScheduled)
        return;

      GameEntity enemy = _enemies.GetSingleEntity();

      if (enemy == null || !enemy.isEnemyTurn)
        return;

      if (enemy.hasCardsPlacedThisTurn && enemy.CardsPlacedThisTurn > 0)
        return;

      _enemyTurnScheduled = true;

      float delay = _gameConfig.EnemyAI.ThinkDelay;
      _timerService.Schedule(delay, () => ExecuteEnemyTurn(enemy));
    }

    private void ExecuteEnemyTurn(GameEntity enemy)
    {
      Debug.Log("[ProcessEnemyTurnSystem] Enemy executing turn");

      if (enemy == null || !enemy.isEnemyTurn)
      {
        Debug.LogWarning("[ProcessEnemyTurnSystem] Enemy is null or not in turn");
        _enemyTurnScheduled = false;
        return;
      }

      if (CanPlaceCard(enemy))
      {
        PlaceEnemyCard(enemy);
        Debug.Log("[ProcessEnemyTurnSystem] Enemy placed card, scheduling EndTurnRequest");

        _timerService.Schedule(_gameConfig.EnemyAI.ThinkDelay, () => {
          Debug.Log("[ProcessEnemyTurnSystem] Creating EndTurnRequest after delay");
          CreateEntity.Request()
            .With(x => x.isEndTurnRequest = true);
        });
      }
      else
      {
        Debug.Log("[ProcessEnemyTurnSystem] Enemy cannot place card, ending turn immediately");
        CreateEntity.Request()
          .With(x => x.isEndTurnRequest = true);
      }

      _enemyTurnScheduled = false;
    }

    private bool CanPlaceCard(GameEntity enemy) =>
      enemy.CardsInHand.Count > 0 && _enemySlots.count > 0 && enemy.CardsPlacedThisTurn < _gameConfig.GameBalance.MaxCardsPlacedPerTurn;

    private void PlaceEnemyCard(GameEntity enemy)
    {
      var randomSlot = _enemySlots.AsEnumerable().PickRandom();
      var cardId = enemy.CardsInHand[0];

      CreateEntity
        .Request()
        .AddPlaceCardRequest(cardId, randomSlot.Id);

      Debug.Log($"[ProcessEnemyTurnSystem] Enemy placing card {cardId} on slot {randomSlot.Id}");
    }
  }
}
