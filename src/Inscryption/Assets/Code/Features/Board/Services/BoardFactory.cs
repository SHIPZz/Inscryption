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
        private readonly GameConfig _gameConfig;

        public BoardFactory(
            GameContext game,
            IIdService idService,
            IAssetsService assetsService,
            IInstantiateService instantiateService,
            ILevelProvider levelProvider,
            IConfigService configService)
        {
            _game = game;
            _idService = idService;
            _assetsService = assetsService;
            _instantiateService = instantiateService;
            _levelProvider = levelProvider;
            _gameConfig = configService.GetConfig<GameConfig>(nameof(GameConfig));
        }

        public List<GameEntity> CreateSlots(int heroId, int enemyId, int lanes = 4)
        {
            var slots = new List<GameEntity>();

            slots.AddRange(CreateSlotsForOwner(heroId, lanes, isHero: true));
            slots.AddRange(CreateSlotsForOwner(enemyId, lanes, isHero: false));

            return
                slots;
        }

        private List<GameEntity> CreateSlotsForOwner(int ownerId, int lanes, bool isHero)
        {
            Vector3 origin = isHero ? _gameConfig.BoardLayout.HeroOrigin : _gameConfig.BoardLayout.EnemyOrigin;
            IReadOnlyList<Vector3> positions = CalculateSlotPositions(lanes, origin);

            var slots = new List<GameEntity>();
        
            for (int i = 0; i < positions.Count; i++)
                slots.Add(CreateSlot(i, ownerId, isHero, positions[i]));

            return slots;
        }

        private IReadOnlyList<Vector3> CalculateSlotPositions(int lanes, Vector3 origin)
        {
            var gridParams = new GridLayoutParams
            {
                Rows = 1,
                Columns = lanes,
                Spacing = _gameConfig.BoardLayout.Spacing,
                Origin = origin
            };

            return PositionCalculator.CalculateGridPositions(gridParams);
        }

        private GameEntity CreateSlot(int lane, int ownerId, bool isHero, Vector3 position)
        {
            GameEntity slot = CreateSlotEntity(lane, ownerId, isHero);
            CreateSlotView(lane, isHero, slot, position);

            return
                slot;
        }

        private GameEntity CreateSlotEntity(int lane, int ownerId, bool isHero)
        {
            GameEntity slot = _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isBoardSlot = true);

            AddSlotProperties(slot, lane, ownerId, isHero);

            return
                slot;
        }

        private void AddSlotProperties(GameEntity slot, int lane, int ownerId, bool isHero)
        {
            slot.AddSlotLane(lane);
            slot.AddSlotOwner(ownerId);
            slot.AddOccupiedBy(-1);
            slot.AddName("slot");
            slot.isStatic = true;
            slot.isHeroOwner = isHero;
            slot.isEnemyOwner = !isHero;
        }

        private void CreateSlotView(int lane, bool isHero, GameEntity slot, Vector3 position)
        {
            SlotEntityView view = InstantiateSlotView(position);
            ConfigureSlotView(view, lane, isHero, position);
            LinkSlotViewToEntity(slot, view, position);
        }

        private SlotEntityView InstantiateSlotView(Vector3 position)
        {
            SlotEntityView prefab = _assetsService.LoadPrefabWithComponent<SlotEntityView>(nameof(SlotEntityView));
            Quaternion rotation = Quaternion.Euler(_gameConfig.BoardLayout.SlotRotation);

            return
                _instantiateService.Instantiate(prefab, position, rotation, _levelProvider.SlotsParent);
        }

        private void ConfigureSlotView(SlotEntityView view, int lane, bool isHero, Vector3 position)
        {
            position.y = 0;
            view.transform.localPosition = position;
            view.name = $"Slot_Lane{lane}_{(isHero ? "Hero" : "Enemy")}";

            Color slotColor = isHero ? _gameConfig.BoardLayout.HeroSlotColor : _gameConfig.BoardLayout.EnemySlotColor;
            view.SetColor(slotColor);
        }

        private void LinkSlotViewToEntity(GameEntity slot, SlotEntityView view, Vector3 position)
        {
            view.EntityBehaviour.SetEntity(slot);
            slot.ReplaceView(view.EntityBehaviour);

            position.y = 0;
            slot.ReplaceWorldPosition(position);
            slot.ReplaceWorldRotation(Quaternion.Euler(_gameConfig.BoardLayout.SlotRotation));
        }
    }
}