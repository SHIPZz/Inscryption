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
        private readonly IInstantiator _instantiator;

        public SystemFactory(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public T Create<T>() where T : ISystem
        {
            return _instantiator.Instantiate<T>();
        }
    }
}