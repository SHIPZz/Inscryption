using Code.Infrastructure.Services;
using Code.Infrastructure.States.StateInfrastructure;

namespace Code.Infrastructure.States.Factory
{
    public class StateFactory : IStateFactory
    {
        private readonly IInstantiateService _instantiateService;
        
        public StateFactory(IInstantiateService instantiateService)
        {
            _instantiateService = instantiateService;
        }

        public T CreateState<T>() where T : class, IState
        {
            return _instantiateService.Instantiate<T>();
        }
    }
}