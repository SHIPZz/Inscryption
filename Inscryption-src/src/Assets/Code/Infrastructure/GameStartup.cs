using System;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Features.Battles;
using Code.Infrastructure.Systems;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public abstract class MonoInitializable : MonoBehaviour, IInitializable
    {
        public abstract void Initialize();
    }
    
    public class GameStartup : MonoInitializable
    {
        private BattleFeature _battleFeature;
        
        [Inject] private ISystemFactory _systemFactory;

        public override void Initialize()
        {
            _battleFeature = _systemFactory.Create<BattleFeature>();
            _battleFeature.Initialize();

            CreateEntity.Empty();

            CreateInputEntity.Empty()
                .isInput = true;
        }

        private void Update()
        {
            _battleFeature.Execute();
            _battleFeature.Cleanup();
        }

        private void OnDestroy()
        {
            _battleFeature.DeactivateReactiveSystems();
            _battleFeature.Cleanup();
            _battleFeature.TearDown();
            _battleFeature = null;
        }
    }
}