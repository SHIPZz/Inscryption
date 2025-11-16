using Entitas;

namespace Code.Features.Board
{
    public static class BoardHelpers
    {
        public static GameEntity FindOppositeSlot(GameContext game, GameEntity slot)
        {
            int lane = slot.SlotLane;
            bool needHeroSide = !slot.isHeroOwner;

            foreach (var s in game.GetEntities(GameMatcher.BoardSlot))
            {
                if (s.SlotLane == lane && s.isHeroOwner == needHeroSide)
                    return s;
            }

            return null;
        }
    }
}


