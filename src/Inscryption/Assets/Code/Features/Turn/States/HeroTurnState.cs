using System.Threading;
using Code.Common.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.States
{
  public class HeroTurnState : IState, IEnterState, IExitableState
  {
    private readonly GameContext _game;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IGroup<GameEntity> _heroes;

    public HeroTurnState(GameContext game, IGameStateMachine gameStateMachine)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _heroes = game.GetGroup(GameMatcher.Hero);
    }

    public async UniTask EnterAsync(CancellationToken cancellationToken = default)
    {
      Debug.Log("[HeroTurnState] Hero turn started");

      GameEntity hero = _heroes.GetSingleEntity();

      if (hero == null)
      {
        Debug.LogError("[HeroTurnState] Hero is null!");
        return;
      }

      Debug.Log($"[HeroTurnState] Setting hero {hero.Id} isHeroTurn = true (was {hero.isHeroTurn})");
      hero.isHeroTurn = true;
      _gameStateMachine.EnterAsync<DrawState, int>(hero.Id, cancellationToken).Forget();
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      Debug.Log("[HeroTurnState] Exiting");
      await UniTask.CompletedTask;
    }
  }
}
