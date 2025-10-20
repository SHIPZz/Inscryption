using System.Collections.Generic;
using Entitas;

namespace Code.Features.View.Systems
{
  public class ProcessSelfInitializedEntityViewRequestsSystem : IExecuteSystem
  {
    private readonly IGroup<GameEntity> _requests;
    private readonly List<GameEntity> _buffer = new(128);

    public ProcessSelfInitializedEntityViewRequestsSystem(GameContext game)
    {
      _requests = game.GetGroup(GameMatcher.SelfInitializeEntityViewRequest);
    }

    public void Execute()
    {
      foreach (GameEntity request in _requests.GetEntities(_buffer))
      {
        request.SelfInitializeEntityViewRequest.Initialize();
        request.Destroy();
      }
    }
  }
}