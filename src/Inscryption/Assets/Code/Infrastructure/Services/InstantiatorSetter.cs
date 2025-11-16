using Zenject;

namespace Code.Infrastructure.Services
{
    public class InstantiatorSetter : IInitializable
    {
        public InstantiatorSetter(DiContainer diContainer, IInstantiateService instantiateService)
        {
            instantiateService.SetInstantiator(diContainer);
        }

        public void Initialize()
        {
        }
    }
}