using System.Collections.Generic;
using Code.Common;
using Code.Common.Extensions;
using Code.Common.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Features.Board.Services
{
    public class BoardFactory : IBoardFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;
        private readonly IConfigService _configService;
        private readonly IAssetsService _assetsService;
        private readonly IInstantiateService _instantiateService;

        public BoardFactory(
            GameContext game, 
            IIdService idService, 
            IConfigService configService,
            IAssetsService assetsService,
            IInstantiateService instantiateService)
        {
            _game = game;
            _idService = idService;
            _configService = configService;
            _assetsService = assetsService;
            _instantiateService = instantiateService;
        }

        public List<GameEntity> CreateSlots(int heroId, int enemyId, int lanes = 4)
        {
            List<GameEntity> slots = new List<GameEntity>();

            for (int lane = 0; lane < lanes; lane++)
            {
                slots.Add(CreateSlot(lane, heroId, true));
                slots.Add(CreateSlot(lane, enemyId, false));
            }

            return slots;
        }

        private GameEntity CreateSlot(int lane, int ownerId, bool isHero)
        {
            GameEntity slot = _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isBoardSlot = true)
                .With(x => x.AddSlotLane(lane))
                .With(x => x.AddSlotOwner(ownerId))
                .With(x => x.AddOccupiedBy(-1));

            CreateSlotView(lane, isHero, slot);

            return slot;
        }

        private void CreateSlotView(int lane, bool isHero, GameEntity slot)
        {
            BoardConfig boardConfig = _configService.GetConfig<BoardConfig>(nameof(BoardConfig));
            
            if (boardConfig == null)
                return;

            Vector3 position = isHero 
                ? boardConfig.GetHeroSlotPosition(lane) 
                : boardConfig.GetEnemySlotPosition(lane);

            position.y = 3f;

            SlotEntityView slotPrefab = _assetsService.LoadPrefabWithComponent<SlotEntityView>(nameof(SlotEntityView));
            
            if (slotPrefab != null)
            {
                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
                SlotEntityView slotView = _instantiateService.Instantiate(slotPrefab, position, rotation);
                
                if (slotView != null && slotView.EntityBehaviour != null)
                {
                    slotView.EntityBehaviour.SetEntity(slot);
                    slotView.name = $"Slot_Lane{lane}_{(isHero ? "Hero" : "Enemy")}";
                    slotView.SetColor(isHero ? new Color(0.3f, 0.7f, 1f, 0.5f) : new Color(1f, 0.3f, 0.3f, 0.5f));
                    slot.ReplaceView(slotView.EntityBehaviour);
                    slot.ReplaceWorldPosition(position);
                    slot.ReplaceWorldRotation(rotation);
                }
            }
            else
            {
                Debug.LogWarning($"[BoardFactory] Slot prefab not found in Addressables: {nameof(SlotEntityView)}");
            }
        }
    }
}

