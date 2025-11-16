using Code.Features.UI.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.UI
{
    public class UIFeature : Feature
    {
        public UIFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<UpdateHealthUISystem>());
            Add(systemFactory.Create<UpdateTurnIndicatorUISystem>());
        }
    }
}
