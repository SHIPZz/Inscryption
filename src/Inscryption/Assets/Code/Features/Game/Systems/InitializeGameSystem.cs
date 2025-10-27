using System.Collections.Generic;
using Code.Common.Random;
using Code.Common.Services;
using Code.Features.Board.Services;
using Code.Features.Cards.Services;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Features.Layout.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Level;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entitas;
using UnityEngine;
namespace Code.Features.Game.Systems
{
    public class InitializeGameSystem : IInitializeSystem
    {
        private readonly GameContext _game;
        private readonly IHeroFactory _heroFactory;
        private readonly IEnemyFactory _enemyFactory;
        private readonly ICardFactory _cardFactory;
        private readonly ICardStackFactory _cardStackFactory;
        private readonly IBoardFactory _boardFactory;
        private readonly IConfigService _configService;
        private readonly IRandomService _randomService;
        private readonly ILevelProvider _levelProvider;
        private GameConfig _gameConfig;
        private IReadOnlyList<Vector3> _heroHandPositions;
        private IReadOnlyList<Vector3> _enemyHandPositions;
        private ICameraProvider _cameraProvider;
        public InitializeGameSystem(
            GameContext game,
            IHeroFactory heroFactory,
            IEnemyFactory enemyFactory,
            ICardFactory cardFactory,
            ICardStackFactory cardStackFactory,
            IBoardFactory boardFactory,
            IConfigService configService,
            IRandomService randomService,
            ILevelProvider levelProvider, ICameraProvider cameraProvider)
        {
            _game = game;
            _heroFactory = heroFactory;
            _enemyFactory = enemyFactory;
            _cardFactory = cardFactory;
            _cardStackFactory = cardStackFactory;
            _boardFactory = boardFactory;
            _configService = configService;
            _randomService = randomService;
            _levelProvider = levelProvider;
            _cameraProvider = cameraProvider;
        }
        public void Initialize()
        {
            Debug.Log("[InitializeGameSystem] Starting game initialization...");
            _gameConfig = _configService.GetConfig<GameConfig>();
            if (_gameConfig == null)
            {
                Debug.LogError("[InitializeGameSystem] GameConfig not found! Using default values.");
                return;
            }
            CalculateHandPositions();
            GameEntity hero = _heroFactory.CreateHero(_gameConfig.BaseHeroHealth);
            Debug.Log($"[InitializeGameSystem] Hero created: ID={hero.Id}, HP={hero.Hp}");
            GameEntity enemy = _enemyFactory.CreateEnemy(_gameConfig.BaseEnemyHealth);
            Debug.Log($"[InitializeGameSystem] Enemy created: ID={enemy.Id}, HP={enemy.Hp}");
            List<GameEntity> slots = _boardFactory.CreateSlots(hero.Id, enemy.Id);
            Debug.Log($"[InitializeGameSystem] Board created: {slots.Count} slots");
            GameEntity commonStack = CreateCardStack();
            Debug.Log($"[InitializeGameSystem] Common card stack created: ID={commonStack.Id}");
            _game.CreateEntity().AddDrawCardFromStackRequest(
                commonStack.Id,
                3,
                hero.Id,
                _heroHandPositions,
                0.5f,
                0.25f,
                _levelProvider.HeroCardParent);
            _game.CreateEntity().AddDrawCardFromStackRequest(
                commonStack.Id,
                3,
                enemy.Id,
                _enemyHandPositions,
                0.5f,
                0.25f,
                _levelProvider.EnemyCardParent);
            Debug.Log("[InitializeGameSystem] Game initialization complete. Hero's turn!");
        }
        private void CalculateHandPositions()
        {
            var arcLayoutParams = new ArcLayoutParams
            {
                Count = _gameConfig.StartingHandSize,
                Origin = _levelProvider.HeroCardParent.position,
                HorizontalSpacing = 0.3f,      
                VerticalCurve = 0.05f,         
                DepthSpacing = 0.02f,          
                AnglePerCard = 5f              
            };
            CardLayoutData[] heroLayout = PositionCalculator.CalculateArcLayout(arcLayoutParams);
            _heroHandPositions = System.Array.ConvertAll(heroLayout, data => data.Position);
            arcLayoutParams.Origin = _levelProvider.EnemyCardParent.position;
            CardLayoutData[] enemyLayout = PositionCalculator.CalculateArcLayout(arcLayoutParams);
            _enemyHandPositions = System.Array.ConvertAll(enemyLayout, data => data.Position);
        }
        private GameEntity CreateCardStack()
        {
            return _cardStackFactory.CreateCardStack(new CardStackFactory.CardStackCreateData(
                cardCount: _gameConfig.DeckSize,
                position: Vector3.zero,
                verticalOffset: _gameConfig.StackVerticalOffset
            ));
        }
    }
}
