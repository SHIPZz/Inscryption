using Zenject;

namespace Code.Infrastructure.Services
{
    public class InstantiatorSetter : IInitializable
    {
        public InstantiatorSetter(DiContainer diContainer, IInstantiateService instantiateService,
            IDiContainerService diContainerService)
        {
            instantiateService.SetInstantiator(diContainer);
            diContainerService.SetDIContainer(diContainer);
        }

        public void Initialize()
        {
        }
    }
}