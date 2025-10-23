using Code.Infrastructure.Services;
using Entitas;
using Zenject;

namespace Code.Infrastructure.Systems
{
    public interface ISystemFactory
    {
        T Create<T>() where T : ISystem;
    }

    public class SystemFactory : ISystemFactory
    {
        private readonly IInstantiateService _instantiator;

        public SystemFactory(IInstantiateService instantiator)
        {
            _instantiator = instantiator;
        }

        public T Create<T>() where T : ISystem
        {
            return _instantiator.Instantiate<T>();
        }
    }
}