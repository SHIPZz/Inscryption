using Code.Common;
using Code.Common.Extensions;
using Code.Common.Services;
using Code.Features.Statuses.Components;

namespace Code.Features.Statuses.Services
{
    public class StatusFactory : IStatusFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;

        public StatusFactory(GameContext game, IIdService idService)
        {
            _game = game;
            _idService = idService;
        }

        public GameEntity CreateStatus(StatusTypeId typeId, int ownerId, int targetId)
        {
            GameEntity status = _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isStatus = true)
                .With(x => x.AddStatusTypeId(typeId))
                .With(x => x.AddStatusOwner(ownerId))
                .With(x => x.AddStatusTarget(targetId));

            switch (typeId)
            {
                case StatusTypeId.Damage:
                    status.isDamageStatus = true;
                    break;
            }

            return status;
        }
    }
}

