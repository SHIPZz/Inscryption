using Code.Features.Statuses.Services;
using Entitas;

namespace Code.Features.Statuses.Components
{
    [Game]
    public class DamageAnimator : IComponent { public IDamageAnimator Value; }

    [Game] public class AttackAnimatorComponent : IComponent { public IAttackAnimator Value; }
}