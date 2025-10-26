using System.Collections.Generic;
using Code.Common;
using Code.Common.Extensions;
using Code.Common.Services;
using Code.Features.Layout.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Level;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Features.Board.Services
{
    public class BoardFactory : IBoardFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;
        private readonly IAssetsService _assetsService;
        private readonly IInstantiateService _instantiateService;
        private readonly ILevelProvider _levelProvider;

        public BoardFactory(
            GameContext game,
            IIdService idService,
            IAssetsService assetsService,
            IInstantiateService instantiateService,
            ILevelProvider levelProvider)
        {
            _game = game;
            _idService = idService;
            _assetsService = assetsService;
            _instantiateService = instantiateService;
            _levelProvider = levelProvider;
        }

        public List<GameEntity> CreateSlots(int heroId, int enemyId, int lanes = 4)
        {
            var slots = new List<GameEntity>();

            var heroGridParams = new GridLayoutParams
            {
                Rows = 1,
                Columns = lanes,
                Spacing = new Vector2(2.2f, 0),
                Origin = new Vector3(0, 0, -2f)
            };
            
            var heroSlotPositions = PositionCalculator.CalculateGridPositions(heroGridParams);

            for (var i = 0; i < heroSlotPositions.Count; i++)
            {
                slots.Add(CreateSlot(i, heroId, true, heroSlotPositions[i]));
            }

            var enemyGridParams = new GridLayoutParams
            {
                Rows = 1,
                Columns = lanes,
                Spacing = new Vector2(2.2f, 0),
                Origin = new Vector3(0, 0, 2f)
            };
            
            var enemySlotPositions = PositionCalculator.CalculateGridPositions(enemyGridParams);

            for (var i = 0; i < enemySlotPositions.Count; i++)
            {
                slots.Add(CreateSlot(i, enemyId, false, enemySlotPositions[i]));
            }

            return slots;
        }

        private GameEntity CreateSlot(int lane, int ownerId, bool isHero, Vector3 position)
        {
            GameEntity slot = _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isBoardSlot = true)
                .With(x => x.AddSlotLane(lane))
                .With(x => x.AddSlotOwner(ownerId))
                .With(x => x.AddOccupiedBy(-1))
                .With(x => x.isStatic = true)
                .With(x => x.isHeroOwner = isHero)
                .With(x => x.isEnemyOwner = !isHero)
                ;

            CreateSlotView(lane, isHero, slot, position);

            return slot;
        }

        private void CreateSlotView(int lane, bool isHero, GameEntity slot, Vector3 position)
        {
            SlotEntityView slotPrefab = _assetsService.LoadPrefabWithComponent<SlotEntityView>(nameof(SlotEntityView));
            Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
            
            SlotEntityView slotView = _instantiateService.Instantiate(slotPrefab, position, rotation, _levelProvider.SlotsParent);

            position.y = 0;
            slotView.transform.localPosition = position;

            slotView.EntityBehaviour.SetEntity(slot);
            slotView.name = $"Slot_Lane{lane}_{(isHero ? "Hero" : "Enemy")}";
            slotView.SetColor(isHero ? new Color(0.3f, 0.7f, 1f, 0.5f) : new Color(1f, 0.3f, 0.3f, 0.5f));

            slot.ReplaceView(slotView.EntityBehaviour);
            slot.ReplaceWorldPosition(position);
            slot.ReplaceWorldRotation(rotation);
        }
    }
}