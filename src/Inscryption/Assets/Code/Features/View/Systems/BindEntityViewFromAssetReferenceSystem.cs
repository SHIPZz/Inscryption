using System;
using System.Collections.Generic;
using Code.Features.View.Factory;
using Entitas;

namespace Code.Features.View.Systems
{
  public class BindEntityViewFromAssetReferenceSystem : IExecuteSystem
  {
    private readonly IEntityViewFactory _entityViewFactory;
    private readonly IGroup<GameEntity> _entities;
    private readonly List<GameEntity> _buffer = new(32);

    public BindEntityViewFromAssetReferenceSystem(Contexts contexts, IEntityViewFactory entityViewFactory)
    {
      _entityViewFactory = entityViewFactory;
      _entities = contexts.game.GetGroup(GameMatcher
        .AllOf(
          GameMatcher.ViewAssetReference)
        .NoneOf(
          GameMatcher.View,
          GameMatcher.LoadingView));
    }

    public void Execute()
    {
      foreach (GameEntity entity in _entities.GetEntities(_buffer))
      {
        if (entity.ViewAssetReference == null)
          throw new NullReferenceException($"{entity} : {nameof(ViewAssetReference)} component value is null");
        
        _entityViewFactory.CreateViewFromAssetReference(entity);
      }
    }
  }
}