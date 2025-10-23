using Code.Common.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Movement
{
    public class MovementFeature : Feature
    {
        public MovementFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<UpdateWorldPositionSystem>());
            Add(systemFactory.Create<UpdateWorldRotationSystem>());
            Add(systemFactory.Create<UpdateParentSystem>());
            Add(systemFactory.Create<UpdateLocalPositionSystem>());
        }
    }
}

