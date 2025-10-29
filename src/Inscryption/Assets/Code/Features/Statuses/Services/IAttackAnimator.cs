using UnityEngine;

namespace Code.Features.Statuses.Services
{
    public interface IAttackAnimator
    {
        void PlayAttackAnimation(Transform target);
    }
}
