using Zenject;

namespace Code.Infrastructure.Services
{
    public interface IDiContainerService
    {
        void SetDIContainer(DiContainer diContainer);
        T Resolve<T>();
        T Instantiate<T>();
    }
}