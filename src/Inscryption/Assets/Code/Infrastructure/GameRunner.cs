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

        private GameRootFeature _gameRootFeature;

        public GameRunner(ISystemFactory systemFactory)
        {
            _systemFactory = systemFactory;
        }

        public void Initialize()
        {
            Debug.Log("=== Game Test Runner Started ===");

            _gameRootFeature = _systemFactory.Create<GameRootFeature>();
            _gameRootFeature.Initialize();

            Debug.Log("=== Game Initialized ===");
        }

        public void Tick()
        {
            _gameRootFeature?.Execute();
            _gameRootFeature?.Cleanup();
        }

        public void Dispose()
        {
            _gameRootFeature?.DeactivateReactiveSystems();
            _gameRootFeature?.TearDown();
            _gameRootFeature = null;

            Debug.Log("=== Game Test Runner Destroyed ===");
        }
    }
}

