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
            Debug.Log("[TestDamageStatusSystem] Creating test entities...");
            
            GameEntity testEntity1 = CreateTestEntity(10);
            CreateDamageStatus(testEntity1.Id, 2);
            
            GameEntity testEntity2 = CreateTestEntity(5);
            CreateDamageStatus(testEntity2.Id, 3);
            
            GameEntity testEntity3 = CreateTestEntity(7);
            CreateDamageStatus(testEntity3.Id, 5);
            
            Debug.Log("[TestDamageStatusSystem] Test setup complete.");
        }

        private GameEntity CreateTestEntity(int hp)
        {
            GameEntity entity = _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.AddHp(hp))
                .With(x => x.AddMaxHp(hp));
            
            Debug.Log($"[TestDamageStatusSystem] Entity created: ID={entity.Id}, HP={hp}");
            return entity;
        }

        private GameEntity CreateDamageStatus(int targetId, int damageValue)
        {
            GameEntity status = _statusFactory.CreateStatus(StatusTypeId.Damage, 0, targetId, damageValue);
            
            Debug.Log($"[TestDamageStatusSystem] Status created: Target={targetId}, Damage={damageValue}");
            return status;
        }
    }
}

