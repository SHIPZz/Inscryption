using System;
using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Features.View.Factory;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.View.Systems
{
  public class BindEntityViewFromAddressableKeySystem : IExecuteSystem
  {
    private readonly IEntityViewFactory _entityViewFactory;
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);

    public BindEntityViewFromAddressableKeySystem(Contexts contexts, IEntityViewFactory entityViewFactory)
    {
      _entityViewFactory = entityViewFactory;
      _entities = contexts.game.GetGroup(GameMatcher
        .AllOf(
          GameMatcher.ViewAddressableKey)
        .NoneOf(
          GameMatcher.View,
          GameMatcher.LoadingView));
    }

    public void Execute()
    {
      foreach (GameEntity entity in _entities.GetEntities(_buffer))
      {
        if (entity.ViewAddressableKey.IsNullOrEmpty())
          throw new NullReferenceException($"{entity} : {nameof(ViewAddressableKey)} component value is null");
        
        _entityViewFactory.CreateViewFromAddressableKey(entity).Forget();
      }
    }
  }
}