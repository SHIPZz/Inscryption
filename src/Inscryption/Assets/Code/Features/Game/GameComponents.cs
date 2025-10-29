using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Features.Game
{
    [Game]
    public class GameEndRequest : IComponent
    {
        public bool HeroWon;
        public int HeroHp;
        public int EnemyHp;
    }
    
    [Game]
    public class GameEnd : IComponent
    {
    }
}
