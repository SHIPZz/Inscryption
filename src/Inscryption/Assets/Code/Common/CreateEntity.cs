using Code.Common.Extensions;

namespace Code.Common
{
  public static class CreateEntity
  {
    public static GameEntity Empty() =>
      Contexts.sharedInstance.game.CreateEntity();

    public static GameEntity Request() =>
      Contexts.sharedInstance.game.CreateEntity()
        .With(x => x.isRequest = true);
  }
}