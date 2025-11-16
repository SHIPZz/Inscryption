using System;

namespace Code.Common.Time
{
  public interface ITimerService
  {
    void Schedule(float delay, Action callback);
  }
}