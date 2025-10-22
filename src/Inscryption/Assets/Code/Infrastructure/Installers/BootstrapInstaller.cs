using Code.Common.Collisions;
using Code.Common.Physics;
using Code.Common.Random;
using Code.Common.Services;
using Code.Common.Time;
using Code.Features.Board.Services;
using Code.Features.Cards.Services;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Features.Statuses.Services;
using Code.Features.View.Factory;
using Code.Features.View.Pool;
using Code.Infrastructure.Services;
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
            Container.BindInterfacesTo<UnityRandomService>().AsSingle();
            
            Container.Bind<IIdService>().To<IdService>().AsSingle();
            Container.Bind<ICameraProvider>().To<CameraProvider>().AsSingle();
            Container.Bind<IRaycastService>().To<RaycastService>().AsSingle();
            Container.Bind<IInputService>().To<UnityInputService>().AsSingle();
            Container.Bind<IStatusFactory>().To<StatusFactory>().AsSingle();
            
            Container.Bind<ICardFactory>().To<CardFactory>().AsSingle();
            Container.Bind<ICardStackFactory>().To<CardStackFactory>().AsSingle();
            Container.Bind<IHeroFactory>().To<HeroFactory>().AsSingle();
            Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
            Container.Bind<IBoardFactory>().To<BoardFactory>().AsSingle();
            Container.Bind<IHeroProvider>().To<HeroProvider>().AsSingle();
            Container.Bind<IEnemyProvider>().To<EnemyProvider>().AsSingle();
            
            Container.Bind<IAssetsService>().To<AssetsService>().AsSingle();
            Container.Bind<IConfigService>().To<ConfigService>().AsSingle();
            Container.Bind<IInstantiateService>().To<InstantiateService>().AsSingle();
            Container.Bind<ICollisionRegistry>().To<CollisionRegistry>().AsSingle();
            Container.Bind<IProjectContext>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IViewPool>().To<ViewPool>().AsSingle();
            Container.Bind<IEntityViewFactory>().To<EntityViewFactory>().AsSingle();
            Container.Bind<IViewFactory>().To<ViewFactory>().AsSingle();
            
            Container.BindInstance(Contexts.sharedInstance).AsSingle();
            Container.BindInstance(Contexts.sharedInstance.game).AsSingle();
            Container.BindInstance(Contexts.sharedInstance.meta).AsSingle();
            Container.BindInstance(Contexts.sharedInstance.input).AsSingle();
        }
    }
}