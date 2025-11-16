using System;
using System.Threading;
using Code.Common.Extensions;
using Code.Features;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.Systems;
using Cysharp.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Code.Infrastructure.States.States
{
    public class GameRunnerState : IEnterState, IUpdateable, IExitableState, IDisposable
    {
        private readonly ISystemFactory _systemFactory;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly GameContext _game;

        private ProjectRootFeature _projectRootFeature;

        public GameRunnerState(ISystemFactory systemFactory, IGameStateMachine gameStateMachine, GameContext game)
        {
            _systemFactory = systemFactory;
            _gameStateMachine = gameStateMachine;
            _game = game;
        }

        public async UniTask EnterAsync(CancellationToken cancellationToken = default)
        {
            _projectRootFeature = _systemFactory.Create<ProjectRootFeature>();
            _projectRootFeature.Initialize();

            await _gameStateMachine.EnterAsync<Features.Turn.States.FirstTurnState>(cancellationToken);

            await UniTask.CompletedTask;
        }

        public async UniTask ExitAsync(CancellationToken cancellationToken = default)
        {
            Cleanup();

            await UniTask.CompletedTask;
        }

        public void Update()
        {
            _projectRootFeature?.Execute();
            _projectRootFeature?.Cleanup();
        }

        public void Dispose()
        {
            Cleanup();
            (_gameStateMachine as IDisposable)?.Dispose();
        }

        private void Cleanup()
        {
            _projectRootFeature?.DeactivateReactiveSystems();
            _projectRootFeature?.TearDown();
            _projectRootFeature = null;
        }
    }
}