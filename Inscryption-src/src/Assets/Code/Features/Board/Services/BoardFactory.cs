using System.Collections.Generic;
using Code.Common;
using Code.Common.Extensions;
using Code.Common.Services;

namespace Code.Features.Board.Services
{
    public class BoardFactory : IBoardFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;

        public BoardFactory(GameContext game, IIdService idService)
        {
            _game = game;
            _idService = idService;
        }

        public List<GameEntity> CreateSlots(int heroId, int enemyId, int lanes = 4)
        {
            List<GameEntity> slots = new List<GameEntity>();

            for (int lane = 0; lane < lanes; lane++)
            {
                slots.Add(CreateSlot(lane, heroId));
                slots.Add(CreateSlot(lane, enemyId));
            }

            return slots;
        }

        private GameEntity CreateSlot(int lane, int ownerId)
        {
            return _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isBoardSlot = true)
                .With(x => x.AddSlotLane(lane))
                .With(x => x.AddSlotOwner(ownerId))
                .With(x => x.AddOccupiedBy(-1));
        }
    }
}

