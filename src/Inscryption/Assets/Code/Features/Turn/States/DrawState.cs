using System.Threading;
using Code.Common;
using Code.Common.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Code.Infrastructure.States.StateInfrastructure;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.States
{
  public class DrawState : IState, IPayloadState<int>, IExitableState
  {
    private readonly GameContext _game;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly GameConfig _gameConfig;

    public DrawState(GameContext game, IGameStateMachine gameStateMachine, IConfigService configService)
    {
      _game = game;
      _gameStateMachine = gameStateMachine;
      _gameConfig = configService.GetConfig<GameConfig>();
    }

    public async UniTask EnterAsync(int playerId, CancellationToken cancellationToken = default)
    {
      GameEntity player = _game.GetEntityWithId(playerId);

      int maxHandSize = _gameConfig.GameBalance.MaxHandSize;
      int cardsToDraw = Mathf.Max(0, maxHandSize - player.CardsInHand.Count);

      if (cardsToDraw > 0)
      {
        for (int i = 0; i < cardsToDraw; i++)
        {
          CreateEntity
            .Request()
            .AddDrawCardRequest(playerId);
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(_gameConfig.AnimationTiming.CardMoveDuration * cardsToDraw), cancellationToken: cancellationToken);
      }

      if (player.isHero)
      {
        _gameStateMachine.EnterAsync<PlacementState, int>(playerId, cancellationToken).Forget();
      }
      else if (player.isEnemy)
      {
        _gameStateMachine.EnterAsync<EnemyPlaceCardsState, int>(playerId, cancellationToken).Forget();
      }
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken = default)
    {
      await UniTask.CompletedTask;
    }
  }
}
