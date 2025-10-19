using System.Collections.Generic;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public List<MonoInitializable> Initializables = new List<MonoInitializable>();
        
        public override void InstallBindings()
        {
            foreach (var initializable in Initializables)
            {
                Container.BindInterfacesAndSelfTo(initializable.GetType()).FromInstance(initializable).AsSingle();
            }
        }
    }
}