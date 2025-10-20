namespace Code.Features.View.Registrars
{
  public interface IEntityComponentRegistrar
  {
    EntityBehaviour EntityBehaviour { get; }
    void RegisterComponents();
    void UnregisterComponents();
  }
}