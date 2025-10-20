using Cysharp.Threading.Tasks;

namespace Code.Features.View.Factory
{
    public interface IEntityViewFactory
    {
        IEntityView CreateViewFromResourcesPath(GameEntity entity);
        IEntityView CreateViewFromPrefab(GameEntity entity);
        UniTask<IEntityView> CreateViewFromAssetReference(GameEntity entity);
        string GetViewPath(GameEntity entity);
        UniTask<IEntityView> CreateViewFromAddressableKey(GameEntity entity);
    }
}