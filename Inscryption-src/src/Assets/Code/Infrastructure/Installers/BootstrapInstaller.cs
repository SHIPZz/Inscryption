using Code.Common.Random;
using Code.Common.Services;
using Code.Common.Time;
using Code.Features.Board.Services;
using Code.Features.Cards.Services;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Features.Statuses.Services;
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
            Container.Bind<IStatusFactory>().To<StatusFactory>().AsSingle();
            
            Container.Bind<ICardFactory>().To<CardFactory>().AsSingle();
            Container.Bind<IHeroFactory>().To<HeroFactory>().AsSingle();
            Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
            Container.Bind<IBoardFactory>().To<BoardFactory>().AsSingle();
            
            Container.BindInstance(Contexts.sharedInstance.game).AsSingle();
            Container.BindInstance(Contexts.sharedInstance.meta).AsSingle();
            Container.BindInstance(Contexts.sharedInstance.input).AsSingle();
        }
    }
}