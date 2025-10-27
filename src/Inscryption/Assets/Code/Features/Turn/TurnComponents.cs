using Entitas;

namespace Code.Features.Turn
{
    [Game]
    public class PlaceCardRequest : IComponent
    {
        public int CardId;
        public int SlotId;
    }

    [Game]
    public class AttackRequest : IComponent
    {
        public int AttackerId;
        public int TargetId;
        public int Damage;
    }

    [Game]
    public class DrawCardRequest : IComponent
    {
        public int PlayerId;
    }

    [Game]
    public class EndTurnRequest : IComponent
    {
    }

    [Game]
    public class AttackPhase : IComponent
    {
    }

    [Game]
    public class SwitchTurnRequest : IComponent
    {
    }

    [Game]
    public class CardsPlacedThisTurn : IComponent
    {
        public int Value;
    }

    public enum TurnPhase
    {
        PlayerPlacement,
        PlayerAttack,
        EnemyWait,
        EnemyPlacement,
        EnemyAttack
    }

    [Game]
    public class CurrentTurnPhase : IComponent
    {
        public TurnPhase Phase;
    }

    [Game]
    public class PhaseTimer : IComponent
    {
        public float ElapsedTime;
        public float TargetDuration;
    }
}

