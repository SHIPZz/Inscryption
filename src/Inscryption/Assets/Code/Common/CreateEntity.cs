namespace Code.Common
{
  public static class CreateEntity
  {
    public static GameEntity Empty() =>
      Contexts.sharedInstance.game.CreateEntity();
  }
}