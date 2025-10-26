using Code.Features.UI.Services;
using Entitas;

namespace Code.Features.UI.Systems
{
    public class InitializeGameHUDSystem : IInitializeSystem
    {
        private readonly MetaContext _meta;
        private readonly IUIProvider _uiProvider;

        public InitializeGameHUDSystem(MetaContext meta, IUIProvider uiProvider)
        {
            _uiProvider = uiProvider;
            _meta = meta;
        }

        public void Initialize()
        {
            _meta.ReplaceGameHUD(_uiProvider.GameHUD);
        }
    }
}