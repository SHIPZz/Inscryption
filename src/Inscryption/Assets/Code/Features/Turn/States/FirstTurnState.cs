using System.Threading;
using Code.Common.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.States
{
  public class FirstTurnState : IState, IEnterState, IExitableState
  {
    private readonly GameContext _game;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IGroup<GameEntity> _heroes;

    public FirstTurnState(GameContext game, IGameStateMachine gameStateMachine)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _heroes = game.GetGroup(GameMatcher.Hero);
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      GameEntity hero = _heroes.GetSingleEntity();

      if (hero != null)
      {
        hero.isHeroTurn = true;
        _gameStateMachine.EnterAsync<PlacementState, int>(hero.Id, cancellationToken).Forget();
      }
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      await UniTask.CompletedTask;
    }
  }
}
