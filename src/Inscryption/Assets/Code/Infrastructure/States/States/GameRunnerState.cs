using System;
using System.Threading;
using Code.Features;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.Systems;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.States.States
{
    public class GameRunnerState : IEnterState, IUpdateable, IExitableState, IDisposable
    {
        private readonly ISystemFactory _systemFactory;

        private ProjectRootFeature _projectRootFeature;

        public GameRunnerState(ISystemFactory systemFactory)
        {
            _systemFactory = systemFactory;
        }

        public async UniTask EnterAsync(CancellationToken cancellationToken = default)
        {
            Debug.Log("=== Game Test Runner Started ===");

            _projectRootFeature = _systemFactory.Create<ProjectRootFeature>();
            _projectRootFeature.Initialize();

            Debug.Log("=== Game Initialized ===");
            
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

            Debug.Log("=== Game Test Runner Destroyed ===");
        }

        private void Cleanup()
        {
            _projectRootFeature?.DeactivateReactiveSystems();
            _projectRootFeature?.TearDown();
            _projectRootFeature = null;
        }
    }
}