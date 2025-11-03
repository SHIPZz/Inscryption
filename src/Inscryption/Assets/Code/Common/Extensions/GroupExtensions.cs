using Entitas;

namespace Code.Common.Extensions
{
  public static class GroupExtensions
  {
    public static GameEntity GetSingleEntity(this IGroup<GameEntity> group)
    {
      foreach (GameEntity entity in group)
        return entity;

      return null;
    }
  }
}
