using System;

namespace Code.Common.Time
{
  public interface ITimeService
  {
    float DeltaTime { get; }
    DateTime UtcNow { get; }
    void StopTime();
    void StartTime();
  }
}