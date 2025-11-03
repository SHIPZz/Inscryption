using System;

namespace Code.Features.Battle.Services
{
  public interface IAttackSchedulerService
  {
    void ScheduleAttack(float delay, int attackerId, int targetId, int damage);
    void ScheduleSwitchTurn(float delay);
  }
}
