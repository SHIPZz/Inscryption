using System;
using Code.Features;
using Code.Infrastructure.Systems;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameTestRunner : ITickable, IDisposable
    {
        private readonly ISystemFactory _systemFactory;

        private float _turnTimer = 0f;
        private const float TurnDelay = 2f;
        private bool _isInitialized = false;
        private GameRootFeature _gameRootFeature;

        public GameTestRunner(ISystemFactory systemFactory)
        {
            _systemFactory = systemFactory;
        }

        public void Initialize()
        {
            Debug.Log("=== Game Test Runner Started ===");

            _gameRootFeature = _systemFactory.Create<GameRootFeature>();
            _gameRootFeature.Initialize();

            _isInitialized = true;

            Debug.Log("=== Game Initialized ===");
        }

        public void Tick()
        {
            if (!_isInitialized)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Contexts.sharedInstance.game.GetGroup(GameMatcher.Enemy).GetSingleEntity().isDestructed = true;
                
                Contexts.sharedInstance.game.CreateEntity()
                    .AddGameEndRequest(true,0,0);
            }
            
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                Contexts.sharedInstance.game.GetGroup(GameMatcher.Hero).GetSingleEntity().isDestructed = true;
                
                Contexts.sharedInstance.game.CreateEntity()
                    .AddGameEndRequest(false,0,0);
            }
            
            
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

