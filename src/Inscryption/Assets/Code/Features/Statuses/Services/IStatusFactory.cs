using Code.Features.Statuses.Components;
using Entitas;

namespace Code.Features.Statuses.Services
{
    public interface IStatusFactory
    {
        GameEntity CreateStatus(StatusTypeId typeId, int ownerId, int targetId, int value);
    }
}

