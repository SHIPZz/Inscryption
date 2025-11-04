using System.Threading;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.States
{
  public class PlacementState : IState, IPayloadState<int>, IUpdateable, IExitableState
  {
    private readonly GameContext _game;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IGroup<GameEntity> _endTurnRequests;
    private readonly System.Collections.Generic.List<GameEntity> _buffer = new(4);

    private int _playerId;
    private bool _isTransitioning;

    public PlacementState(GameContext game, IGameStateMachine gameStateMachine)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _endTurnRequests = game.GetGroup(GameMatcher.EndTurnRequest);
    }

    public async UniTask EnterAsync(int playerId, CancellationToken cancellationToken = default)
    {
      _playerId = playerId;
      _isTransitioning = false;
      Debug.Log($"[PlacementState] Player {playerId} can place cards. Waiting for EndTurn...");

      await UniTask.CompletedTask;
    }

    public void Update()
    {
      if (_isTransitioning)
        return;

      if (_endTurnRequests.count > 0)
      {
        Debug.Log($"[PlacementState] EndTurnRequest detected for player {_playerId}, transitioning to AttackState");
        _isTransitioning = true;

        foreach (GameEntity request in _endTurnRequests.GetEntities(_buffer))
          request.Destroy();

        _gameStateMachine.EnterAsync<AttackState, int>(_playerId).Forget();
      }
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      Debug.Log("[PlacementState] Exiting");
      await UniTask.CompletedTask;
    }
  }
}
