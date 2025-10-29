using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Common.Destruct
{
    public class FinalizeCardDestructionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _cardsToFinalize;
        private readonly IGroup<GameEntity> _boardSlots;
        private readonly List<GameEntity> _buffer = new(64);

        public FinalizeCardDestructionSystem(GameContext game)
        {
            _cardsToFinalize = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Card, GameMatcher.Destructed));
            _boardSlots = game.GetGroup(GameMatcher.AllOf(GameMatcher.BoardSlot));
        }

        public void Execute()
        {
            foreach (GameEntity card in _cardsToFinalize.GetEntities(_buffer))
            {
                if (card.isOnBoard && card.hasLane)
                {
                    foreach (GameEntity slot in _boardSlots)
                    {
                        if (slot.OccupiedBy == card.Id)
                        {
                            slot.ReplaceOccupiedBy(-1);
                            Debug.Log($"[FinalizeCardDestructionSystem] Slot {slot.Id} freed (lane {slot.SlotLane})");
                            break;
                        }
                    }
                }
            }
        }
    }
}


