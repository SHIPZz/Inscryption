using System;
using Code.Features;
using Code.Infrastructure.Systems;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameRunner : ITickable, IDisposable
    {
        private readonly ISystemFactory _systemFactory;

        private ProjectRootFeature _projectRootFeature;

        public GameRunner(ISystemFactory systemFactory)
        {
            _systemFactory = systemFactory;
        }

        public void Initialize()
        {
            Debug.Log("=== Game Test Runner Started ===");

            _projectRootFeature = _systemFactory.Create<ProjectRootFeature>();
            _projectRootFeature.Initialize();

            Debug.Log("=== Game Initialized ===");
        }

        public void Tick()
        {
            _projectRootFeature?.Execute();
            _projectRootFeature?.Cleanup();
        }

        public void Dispose()
        {
            _projectRootFeature?.DeactivateReactiveSystems();
            _projectRootFeature?.TearDown();
            _projectRootFeature = null;

            Debug.Log("=== Game Test Runner Destroyed ===");
        }
    }
}

