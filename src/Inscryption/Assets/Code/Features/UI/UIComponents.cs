using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Features.UI
{
    [Meta, Unique]
    public class GameHUDComponent : IComponent
    {
        public GameHUD Value;
    }

    [Meta]
    public class UpdateHealthUIRequest : IComponent
    {
    }

    [Meta]
    public class UpdateTurnUIRequest : IComponent
    {
    }
}
