using Code.Common;
using Code.Common.Extensions;
using Code.Common.Time;

namespace Code.Features.Battle.Services
{
  public class AttackSchedulerService : IAttackSchedulerService
  {
    private readonly ITimerService _timerService;

    public AttackSchedulerService(ITimerService timerService)
    {
      _timerService = timerService;
    }

    public void ScheduleAttack(float delay, int attackerId, int targetId, int damage)
    {
      _timerService.Schedule(delay, () =>
        CreateEntity
          .Request()
          .AddAttackRequest(attackerId, targetId, damage));
    }

    public void ScheduleSwitchTurn(float delay)
    {
      _timerService.Schedule(delay, () =>
        CreateEntity
          .Request()
          .With(x => x.isSwitchTurnRequest = true));
    }
  }
}
