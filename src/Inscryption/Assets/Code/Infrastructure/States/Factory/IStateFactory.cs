using Code.Infrastructure.States.StateInfrastructure;

namespace Code.Infrastructure.States.Factory
{
  public interface IStateFactory
  {
    T CreateState<T>() where T : class, IState;
  }
}