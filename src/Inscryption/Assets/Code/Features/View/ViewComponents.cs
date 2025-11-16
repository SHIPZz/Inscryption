using Code.Features.View.SelfInitialized;
using Entitas;
using UnityEngine.AddressableAssets;

namespace Code.Features.View
{
    [Game] public class View : IComponent { public IEntityView Value; }
    [Game] public class ViewPath : IComponent { public string Value; }
    [Game] public class ViewPrefab : IComponent { public EntityBehaviour Value; }
    [Game] public class ViewAssetReference : IComponent { public AssetReference Value; }
    [Game] public class ViewAddressableKey : IComponent { public string Value; }
    [Game] public class LoadingView : IComponent { }
    [Game] public class SelfInitializeEntityViewRequest : IComponent { public AbstractSelfInitializedEntityView Value; }
    [Game] public class SelfInitializedView : IComponent { }

}