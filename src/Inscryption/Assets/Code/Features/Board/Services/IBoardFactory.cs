using System.Collections.Generic;

namespace Code.Features.Board.Services
{
    public interface IBoardFactory
    {
        List<GameEntity> CreateSlots(int heroId, int enemyId, int lanes = 4);
    }
}

