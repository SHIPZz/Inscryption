using System.Linq;
using System.Threading;
using Code.Common;
using Code.Common.Extensions;
using Code.Features.Board.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.States
{
  public class EnemyPlaceCardsState : IState, IPayloadState<int>, IExitableState
  {
    private readonly GameContext _game;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IGroup<GameEntity> _slots;

    public EnemyPlaceCardsState(GameContext game, IGameStateMachine gameStateMachine)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _slots = game.GetGroup(GameMatcher.BoardSlot);
    }

    public async UniTask EnterAsync(int enemyId, CancellationToken cancellationToken = default)
    {
      Debug.Log($"[EnemyPlaceCardsState] Enemy {enemyId} placing cards automatically");

      GameEntity enemy = _game.GetEntityWithId(enemyId);
      if (enemy == null)
      {
        Debug.LogError($"[EnemyPlaceCardsState] Enemy {enemyId} not found!");
        _gameStateMachine.EnterAsync<AttackState, int>(enemyId, cancellationToken).Forget();
        return;
      }

      PlaceEnemyCards(enemy);

      await UniTask.Delay(System.TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);

      _gameStateMachine.EnterAsync<AttackState, int>(enemyId, cancellationToken).Forget();
    }

    private void PlaceEnemyCards(GameEntity enemy)
    {
      var enemySlots = _slots.GetOwnedSlots(enemy.Id).ToList();
      var cardsInHand = enemy.CardsInHand.ToList();

      if (!cardsInHand.Any() || !enemySlots.Any())
      {
        Debug.Log($"[EnemyPlaceCardsState] Enemy has {cardsInHand.Count} cards and {enemySlots.Count} slots");
        return;
      }

      foreach (var slot in enemySlots)
      {
        if (!cardsInHand.Any())
          break;

        if (slot.hasOccupiedBy)
          continue;

        int cardId = cardsInHand.First();
        cardsInHand.Remove(cardId);

        Debug.Log($"[EnemyPlaceCardsState] Creating PlaceCardRequest for enemy card {cardId} to slot {slot.Id}");

        CreateEntity
          .Request()
          .AddPlaceCardRequest(cardId, slot.Id);
      }
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      Debug.Log("[EnemyPlaceCardsState] Exiting");
      await UniTask.CompletedTask;
    }
  }
}