using Zenject;

namespace Code.Infrastructure.Services
{
    public class DiContainerService : IDiContainerService
    {
        private DiContainer _diContainer;

        public DiContainerService(DiContainer diContainer)
        {
            SetDIContainer(diContainer);
        }

        public void SetDIContainer(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public T Resolve<T>()
        {
            return _diContainer.Resolve<T>();
        }

        public T Instantiate<T>()
        {
            return _diContainer.Instantiate<T>();
        }
    }
}