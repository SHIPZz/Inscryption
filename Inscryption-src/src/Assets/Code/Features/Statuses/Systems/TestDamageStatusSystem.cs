using Code.Common;
using Code.Common.Extensions;
using Code.Common.Services;
using Code.Features.Statuses.Components;
using Code.Features.Statuses.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Statuses.Systems
{
    public class TestDamageStatusSystem : IInitializeSystem
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;
        private readonly IStatusFactory _statusFactory;

        public TestDamageStatusSystem(GameContext game, IIdService idService, IStatusFactory statusFactory)
        {
            _game = game;
            _idService = idService;
            _statusFactory = statusFactory;
        }

        public void Initialize()
        {
            Debug.Log("[TestDamageStatusSystem] Starting test initialization...");
            
            GameEntity testEntity = CreateTestEntity();
            Debug.Log($"[TestDamageStatusSystem] Test entity created with ID: {testEntity.Id}, HP: {testEntity.Hp}, MaxHP: {testEntity.MaxHp}");
            
            GameEntity damageStatus = CreateDamageStatus(testEntity.Id);
            Debug.Log($"[TestDamageStatusSystem] Damage status created with ID: {damageStatus.Id}, Target: {damageStatus.StatusTarget}, TypeId: {damageStatus.StatusTypeId}");
            
            Debug.Log("[TestDamageStatusSystem] Test initialization complete. Waiting for ApplyDamageStatusSystem to process...");
        }

        private GameEntity CreateTestEntity()
        {
            Debug.Log("[TestDamageStatusSystem] Creating test entity...");
            
            GameEntity entity = _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.AddHp(5))
                .With(x => x.AddMaxHp(5));
            
            return entity;
        }

        private GameEntity CreateDamageStatus(int targetId)
        {
            Debug.Log($"[TestDamageStatusSystem] Creating damage status for target ID: {targetId}");
            
            GameEntity status = _statusFactory.CreateStatus(StatusTypeId.Damage, 0, targetId);
            
            return status;
        }
    }
}

