using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Features.Game
{
    [Game, Event(EventTarget.Self)]
    public class GameEndRequest : IComponent
    {
        public bool HeroWon;
        public int HeroHp;
        public int EnemyHp;
    }
}
