using Code.Common.Services;
using Code.Features.Statuses.Services;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Systems;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISystemFactory>().To<SystemFactory>().AsSingle();
            Container.BindInterfacesTo<UnityTimeService>().AsSingle();
            
            Container.Bind<IIdService>().To<IdService>().AsSingle();
            Container.Bind<IStatusFactory>().To<StatusFactory>().AsSingle();
            
            Container.BindInstance(Contexts.sharedInstance.game).AsSingle();
            Container.BindInstance(Contexts.sharedInstance.meta).AsSingle();
            Container.BindInstance(Contexts.sharedInstance.input).AsSingle();
        }
    }
}