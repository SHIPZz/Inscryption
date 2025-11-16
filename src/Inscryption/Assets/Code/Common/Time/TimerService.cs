using System;
using System.Collections.Generic;
using Zenject;

namespace Code.Common.Time
{
  public class TimerService : ITimerService, ITickable
  {
    private readonly ITimeService _timeService;
    private readonly List<ScheduledTimer> _timers = new();
    private readonly List<int> _indicesToRemove = new();

    public TimerService(ITimeService timeService)
    {
      _timeService = timeService;
    }

    public void Schedule(float delay, Action callback)
    {
      _timers.Add(new ScheduledTimer(delay, callback));
    }

    public void Tick()
    {
      for (int i = 0; i < _timers.Count; i++)
      {
        ScheduledTimer timer = _timers[i];
        timer.TimeLeft -= _timeService.DeltaTime;

        if (timer.TimeLeft <= 0)
        {
          timer.Callback?.Invoke();
          _indicesToRemove.Add(i);
        }
        else
        {
          _timers[i] = timer;
        }
      }

      for (int i = _indicesToRemove.Count - 1; i >= 0; i--)
        _timers.RemoveAt(_indicesToRemove[i]);

      _indicesToRemove.Clear();
    }

    private struct ScheduledTimer
    {
      public float TimeLeft;
      public Action Callback;

      public ScheduledTimer(float timeLeft, Action callback)
      {
        TimeLeft = timeLeft;
        Callback = callback;
      }
    }
  }
}