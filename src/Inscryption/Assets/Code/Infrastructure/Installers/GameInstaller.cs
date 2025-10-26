using System.Collections.Generic;
using Code.Infrastructure.Level;
using Code.Infrastructure.Services;
using Zenject;
using Code.Features.Board.Services;
using Code.Features.Cards.Services;
using Code.Features.Layout.Services;
using Code.Features.UI.Services;

namespace Code.Infrastructure.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public LevelProvider LevelProvider;
        public UIProvider UIProvider;
        public List<MonoInitializable> Initializables = new List<MonoInitializable>();

        public override void InstallBindings()
        {
            Container.Bind<ILevelProvider>().FromInstance(LevelProvider).AsSingle();
            Container.Bind<IUIProvider>().FromInstance(UIProvider).AsSingle();

            Container.Bind<IBoardFactory>().To<BoardFactory>().AsSingle();
            Container.Bind<ICardFactory>().To<CardFactory>().AsSingle();
            Container.Bind<ICardStackFactory>().To<CardStackFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<EntryPoint>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameTestRunner>().AsSingle();
            Container.BindInterfacesAndSelfTo<InstantiatorSetter>().AsSingle();

            foreach (var initializable in Initializables)
            {
                Container.BindInterfacesAndSelfTo(initializable.GetType()).FromInstance(initializable).AsSingle();
            }
        }
    }
}