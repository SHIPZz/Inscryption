using System;
using Code.Infrastructure.States.StateMachine;
using Zenject;

namespace Code.Features.Turn.StateMachine
{
  public interface IGameStateMachine : IStateMachine, ITickable, IDisposable
  {
  }
}
