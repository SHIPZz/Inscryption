# Архитектура проекта Inscryption Clone

## Обзор

Проект построен на **ECS (Entity-Component-System)** архитектуре с использованием фреймворка **Entitas** и **Zenject** для dependency injection.

## Основные принципы

### 1. Atomic Components
**Один компонент = одно значение**

```csharp
[Game] public class Hp : IComponent { public int Value; }
[Game] public class Damage : IComponent { public int Value; }
```

### 2. Marker Components
Компоненты без полей для маркировки сущностей:

```csharp
[Game] public class Hero : IComponent { }
[Game] public class Card : IComponent { }
[Game] public class Destructed : IComponent { }
```

### 3. Feature-Based Organization
Код организован по игровым фичам, а не по типам:

```
Features/
  ├── Cards/
  │   ├── CardComponents.cs
  │   ├── CardFeature.cs
  │   ├── Systems/
  │   └── Services/
  ├── Hero/
  ├── Enemy/
  └── Battle/
```

### 4. Deferred Reactivity
Взаимодействие через Request-компоненты, которые живут один кадр:

```csharp
[Game] public class PlaceCardRequest : IComponent { 
    public int CardId; 
    public int SlotId; 
}
```

### 5. Factory Pattern с Functional Extensions
Создание энтити через фабрики с цепочкой `.With()`:

```csharp
return _game.CreateEntity()
    .AddId(_idService.Next())
    .With(x => x.isCard = true)
    .With(x => x.AddHp(hp))
    .With(x => x.AddDamage(damage));
```

---

## Структура проекта

### Common/ - Базовые компоненты и сервисы

#### Компоненты (CommonComponents.cs)
- `Id` - уникальный идентификатор энтити (с индексом)
- `Hp` - текущее здоровье
- `MaxHp` - максимальное здоровье
- `Damage` - урон
- `Destructed` - маркер удаления
- `Active` - маркер активности

#### Сервисы
- **IIdService** - генерация уникальных ID
- **IRandomService** - случайные числа (обертка Unity Random)
- **ITimeService** - время (обертка Unity Time)

#### Extensions
- **FunctionalExtensions** - `.With()` для цепочки вызовов
- **EnumerableExtensions** - работа с коллекциями

---

## Игровые фичи

### 1. Cards - Карты

#### Компоненты (CardComponents.cs)
- `Card` - маркер карты
- `CardOwner` - ID владельца (Hero/Enemy)
- `InHand` - карта в руке
- `OnBoard` - карта на поле
- `Lane` - линия (0-3)

#### Системы
- **ProcessDrawCardRequestSystem** - обработка запросов на взятие карт
  - Обрабатывает `DrawCardRequest(playerId)`
  - Проверяет лимит руки (5 карт)
  - Перемещает карты из колоды в руку
  - Удаляет обработанные Request'ы

#### Фабрика (ICardFactory)
```csharp
GameEntity CreateCard(int hp, int damage, int ownerId);
GameEntity CreateRandomCard(int ownerId);
```

---

### 2. Hero - Герой игрока

#### Компоненты (HeroComponents.cs)
- `Hero` - маркер героя
- `HeroTurn` - маркер хода героя
- `CardsInHand` - список ID карт в руке

#### Фабрика (IHeroFactory)
```csharp
GameEntity CreateHero(int baseHealth);
```

Создает героя с:
- HP = baseHealth
- HeroTurn = true (начинает первым)
- Пустая рука карт

---

### 3. Enemy - Враг

#### Компоненты (EnemyComponents.cs)
- `Enemy` - маркер врага
- `EnemyTurn` - маркер хода врага

#### Системы
- **EnemyAISystem** - искусственный интеллект врага
  - Проверяет отсутствие `EndTurnRequest` и `PlaceCardRequest`
  - Выбирает случайную карту из руки
  - Выбирает случайный свободный слот (используя `ListPool` для оптимизации)
  - Создает `PlaceCardRequest`
  - Создает `EndTurnRequest` при условиях завершения хода

#### Фабрика (IEnemyFactory)
```csharp
GameEntity CreateEnemy(int baseHealth);
```

---

### 4. Board - Игровое поле

#### Компоненты (BoardComponents.cs)
- `BoardSlot` - маркер слота
- `SlotLane` - линия слота (0-3)
- `SlotOwner` - владелец слота (Hero/Enemy ID)
- `OccupiedBy` - ID карты в слоте (-1 = пусто)

#### Системы
- **ProcessPlaceCardRequestSystem** - размещение карт на поле
  - Обрабатывает `PlaceCardRequest(cardId, slotId)`
  - Валидация (слот пустой, карта в руке, лимит 1/ход)
  - Перемещение карты из руки на поле
  - Обновление слота
  - Удаляет обработанные Request'ы

#### Фабрика (IBoardFactory)
```csharp
List<GameEntity> CreateSlots(int heroId, int enemyId, int lanes = 4);
```

Создает слоты для указанного количества линий:
- По умолчанию 4 линии (lanes = 4)
- Создаёт 2 слота на линию (героя + врага)
- Итого: lanes * 2 слотов
- Пример: 4 линии = 8 слотов (4 героя + 4 врага)

---

### 5. Battle - Боевая система

#### Системы

**ProcessAttackPhaseSystem** - фаза атаки
- Обрабатывает маркер `AttackPhase`
- Проверяет отсутствие `SwitchTurnRequest` перед обработкой
- Находит все карты активного игрока на поле
- Для каждой карты ищет цель:
  - Карта врага на той же линии → атакует её
  - Нет карты → атакует игрока напрямую
- Создает `AttackRequest` для каждой атаки
- Создает `SwitchTurnRequest` после всех атак
- Удаляет ВСЕ маркеры `AttackPhase`

**ProcessAttackRequestSystem** - обработка атак
- Обрабатывает `AttackRequest(attackerId, targetId, damage)`
- Применяет урон к цели: `target.Hp -= damage`
- Уменьшает HP на Damage
- Удаляет карты/игроков с HP ≤ 0
- Освобождает слоты от уничтоженных карт
- Удаляет обработанные Request'ы

**CheckVictorySystem** - проверка победы
- Проверяет HP героя и врага
- При HP ≤ 0 объявляет победителя
- Завершает игру

---

### 6. Turn - Система ходов

#### Компоненты (TurnComponents.cs)

**Request компоненты:**
- `PlaceCardRequest` - запрос на размещение карты
- `AttackRequest` - запрос на атаку
- `DrawCardRequest` - запрос на взятие карты
- `EndTurnRequest` - запрос на завершение хода
- `SwitchTurnRequest` - запрос на переключение хода
- `AttackPhase` - маркер фазы атаки

**Состояние:**
- `CardsPlacedThisTurn` - счетчик размещенных карт за ход

#### Системы

**ProcessEndTurnRequestSystem** - обработка завершения хода
- Обрабатывает `EndTurnRequest`
- Проверяет отсутствие `AttackPhase` перед обработкой
- Создает маркер `AttackPhase`
- Удаляет ВСЕ `EndTurnRequest`

**ProcessSwitchTurnRequestSystem** - переключение ходов
- Обрабатывает `SwitchTurnRequest`
- Переключает HeroTurn ↔ EnemyTurn
- Сбрасывает CardsPlacedThisTurn обоим игрокам
- Создает `DrawCardRequest` для следующего игрока
- Использует флаг `turnSwitched` для однократного переключения
- Удаляет ВСЕ `SwitchTurnRequest`

---

### 7. Game - Управление игрой

#### Системы

**InitializeGameSystem** - инициализация игры
- Создает героя (HP=20)
- Создает врага (HP=20)
- Создает игровое поле (8 слотов)
- Создает колоду (30 карт)
- Раздает стартовые руки (3 карты)

**CheckVictorySystem** - проверка победы
- Отслеживает HP игроков
- Завершает игру при победе/поражении

---

### 8. Statuses - Система статусов

#### Компоненты (StatusTypeIdComponent.cs)
- `StatusTypeId` - тип статуса (Damage, Buff, Debuff)
- `Status` - маркер статуса
- `StatusOwner` - кто создал статус
- `StatusTarget` - на кого направлен
- `StatusValue` - значение эффекта
- `DamageStatus` - маркер урона

#### Системы
- **ApplyDamageStatusSystem** - применение статуса урона
- **TestDamageStatusSystem** - тестовая система

#### Фабрика (IStatusFactory)
```csharp
GameEntity CreateStatus(
    StatusTypeId typeId, 
    int ownerId, 
    int targetId, 
    int value
);
```

---

## Иерархия Features

```
GameplayRootFeature
├── GameFeature
│   ├── InitializeGameSystem (Initialize)
│   └── CheckVictorySystem (Execute)
├── CardFeature
│   └── ProcessDrawCardRequestSystem (Execute)
├── HeroFeature
│   (пусто - для будущих систем)
├── EnemyFeature
│   └── EnemyAISystem (Execute)
├── BoardFeature
│   └── ProcessPlaceCardRequestSystem (Execute)
├── TurnFeature
│   ├── ProcessEndTurnRequestSystem (Execute)
│   └── ProcessSwitchTurnRequestSystem (Execute)
└── BattleFeature
    ├── ProcessAttackPhaseSystem (Execute)
    └── ProcessAttackRequestSystem (Execute)
```

**Порядок выполнения систем:**
1. Initialize системы (один раз)
2. Execute системы (каждый кадр)
3. Cleanup системы (удаление Destructed)

---

## Dependency Injection (Zenject)

### BootstrapInstaller

Регистрирует все сервисы и фабрики:

```csharp
// Системы
Container.Bind<ISystemFactory>().To<SystemFactory>().AsSingle();

// Сервисы
Container.BindInterfacesTo<UnityTimeService>().AsSingle();
Container.BindInterfacesTo<UnityRandomService>().AsSingle();
Container.Bind<IIdService>().To<IdService>().AsSingle();

// Фабрики
Container.Bind<ICardFactory>().To<CardFactory>().AsSingle();
Container.Bind<IHeroFactory>().To<HeroFactory>().AsSingle();
Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
Container.Bind<IBoardFactory>().To<BoardFactory>().AsSingle();
Container.Bind<IStatusFactory>().To<StatusFactory>().AsSingle();

// Контексты Entitas
Container.BindInstance(Contexts.sharedInstance.game).AsSingle();
Container.BindInstance(Contexts.sharedInstance.meta).AsSingle();
Container.BindInstance(Contexts.sharedInstance.input).AsSingle();
```

---

## Кодогенерация (Jenny)

### Что генерируется

Для каждого `IComponent`:

```csharp
// Методы добавления
entity.AddHp(10);
entity.ReplaceHp(15);
entity.RemoveHp();

// Проверки
entity.hasHp;
entity.Hp; // property getter

// Матчеры для групп
GameMatcher.Hp;
GameMatcher.AllOf(GameMatcher.Hp, GameMatcher.Card);
```

### Запуск генерации

```bash
cd Jenny
.\Jenny-Gen.bat
```

Или из папки проекта Unity:
```bash
..\..\Jenny\Jenny-Gen.bat
```

---

## Жизненный цикл игры

### 1. Инициализация (Start)
```
GameTestRunner.Start()
  └─> GameplayRootFeature.Initialize()
      └─> InitializeGameSystem.Initialize()
          ├─> HeroFactory.CreateHero(20)
          ├─> EnemyFactory.CreateEnemy(20)
          ├─> BoardFactory.CreateSlots(heroId, enemyId, lanes: 4)
          ├─> CardFactory.CreateRandomCard() x30
          └─> Раздача 3 карт
```

### 2. Игровой цикл (Update)

**Ход героя:**
```
[Игрок] Нажимает 1-4 → PlaceCardRequest
  └─> PlaceCardSystem.Execute()
      └─> Карта перемещается на поле

[Игрок] Нажимает Space → EndTurnRequest
  └─> ProcessEndTurnRequestSystem.Execute()
      └─> Создает AttackPhase
  └─> ProcessAttackPhaseSystem.Execute()
      └─> Создает AttackRequest для каждой карты
      └─> Создает SwitchTurnRequest
  └─> ProcessAttackRequestSystem.Execute()
      └─> Применяет урон
  └─> CheckVictorySystem.Execute()
      └─> Проверяет условия победы
  └─> ProcessSwitchTurnRequestSystem.Execute()
      └─> Переключает ход на врага
      └─> Создает DrawCardRequest для врага
  └─> ProcessDrawCardRequestSystem.Execute()
      └─> Враг берет карту
```

**Ход врага (через 2 секунды):**
```
EnemyAISystem.Execute()
  ├─> IF нет EndTurnRequest И нет PlaceCardRequest:
  │   ├─> Выбирает случайную карту и слот
  │   └─> PlaceCardRequest
  └─> При условиях → EndTurnRequest
    └─> (Цикл повторяется как у героя)
```

### 3. Завершение (OnDestroy)
```
GameTestRunner.OnDestroy()
  └─> GameplayRootFeature.TearDown()
      └─> Очистка всех систем
```

---

## Правила работы с ECS

### ✅ DO (Делать)

1. **Один компонент = одна ответственность**
   ```csharp
   [Game] public class Hp : IComponent { public int Value; }
   [Game] public class MaxHp : IComponent { public int Value; }
   ```

2. **Использовать .With() в фабриках**
   ```csharp
   entity.AddId(id)
       .With(x => x.AddHp(hp))
       .With(x => x.AddMaxHp(hp));
   ```

3. **Request компоненты для событий**
   ```csharp
   _game.CreateEntity().AddPlaceCardRequest(cardId, slotId);
   ```

4. **Проверять существование Request перед созданием**
   ```csharp
   if (_endTurnRequests.count > 0)
       return; // Не создавать новый!
   ```

5. **Обрабатывать первый, удалять ВСЕ**
   ```csharp
   bool processed = false;
   foreach (GameEntity request in _requests.GetEntities(_buffer))
   {
       if (!processed) {
           // Обработка
           processed = true;
       }
       request.isDestructed = true; // Удалить ВСЕ
   }
   ```

6. **Destructed для удаления**
   ```csharp
   entity.isDestructed = true;
   ```

7. **Feature-based организация**
   ```
   Features/Cards/
     ├── CardComponents.cs
     ├── CardFeature.cs
     ├── Systems/
     └── Services/
   ```

8. **Использовать ListPool для временных списков**
   ```csharp
   List<GameEntity> slots = ListPool<GameEntity>.Get();
   // ... использование ...
   ListPool<GameEntity>.Release(slots);
   ```

### ❌ DON'T (Не делать)

1. **Не смешивать данные в одном компоненте**
   ```csharp
   // ПЛОХО
   [Game] public class UnitStats { 
       public int Hp; 
       public int Damage; 
       public int Speed; 
   }
   ```

2. **Не хранить бизнес-логику в компонентах**
   ```csharp
   // ПЛОХО
   public class Card : IComponent {
       public void Attack(Entity target) { ... }
   }
   ```

3. **Не забывать запускать Jenny после изменений**
   - Изменили компоненты → запустить Jenny

4. **Не биндить фичи в Zenject**
   - Фичи создаются через ISystemFactory

5. **Не удалять энтити напрямую**
   ```csharp
   // ПЛОХО
   entity.Destroy();
   
   // ХОРОШО
   entity.isDestructed = true;
   ```

---

## Тестирование

### GameTestRunner

MonoBehaviour для запуска и тестирования игры:

**Управление:**
- `1/2/3/4` - разместить карту на линии 0-3
- `Space` - завершить ход
- Автоход врага через 2 секунды

**Логирование:**
- Создание энтити
- Размещение карт
- Атаки и урон
- Смена ходов
- Победа/поражение

---

## Расширение системы

### Добавление нового компонента

1. Создать компонент:
   ```csharp
   [Game] public class Shield : IComponent { public int Value; }
   ```

2. Запустить Jenny:
   ```bash
   cd Jenny && .\Jenny-Gen.bat
   ```

3. Использовать сгенерированные методы:
   ```csharp
   entity.AddShield(5);
   if (entity.hasShield) { ... }
   ```

### Добавление новой системы

1. Создать класс системы:
   ```csharp
   public class ApplyShieldSystem : IExecuteSystem
   {
       private readonly GameContext _game;
       
       public ApplyShieldSystem(GameContext game) {
           _game = game;
       }
       
       public void Execute() { ... }
   }
   ```

2. Добавить в соответствующую Feature:
   ```csharp
   public class BattleFeature : Feature
   {
       public BattleFeature(ISystemFactory systemFactory)
       {
           Add(systemFactory.Create<ApplyShieldSystem>());
           // ...
       }
   }
   ```

### Добавление новой Feature

1. Создать Feature класс:
   ```csharp
   public class BuffFeature : Feature
   {
       public BuffFeature(ISystemFactory systemFactory)
       {
           Add(systemFactory.Create<ApplyBuffSystem>());
       }
   }
   ```

2. Добавить в GameplayRootFeature:
   ```csharp
   Add(systemFactory.Create<BuffFeature>());
   ```

### Изменение количества линий

Количество линий на поле настраивается через параметр `lanes`:

```csharp
// По умолчанию 4 линии
List<GameEntity> slots = _boardFactory.CreateSlots(heroId, enemyId);

// Кастомное количество (например, 6 линий)
List<GameEntity> slots = _boardFactory.CreateSlots(heroId, enemyId, lanes: 6);
```

**Примечание**: При изменении количества линий убедитесь, что:
- UI поддерживает нужное количество
- Правила размещения карт учитывают новое количество
- EnemyAI корректно работает с изменённым полем

---

## Производительность

### Оптимизации

1. **Буферы для групп**
   ```csharp
   private readonly List<GameEntity> _buffer = new(32);
   group.GetEntities(_buffer); // Без аллокаций
   ```

2. **Индексы для быстрого поиска**
   ```csharp
   [Game, Unique] 
   public class Id : IComponent { 
       [PrimaryEntityIndex] public int Value; 
   }
   
   // Быстрый поиск O(1)
   _game.GetEntityWithId(id);
   ```

3. **Reactive Systems только когда нужно**
   - IExecuteSystem для постоянной проверки
   - IReactiveSystem для реакции на изменения

---

## Полезные ссылки

- **Entitas Wiki**: https://github.com/sschmid/Entitas/wiki
- **Zenject**: https://github.com/modesttree/Zenject
- **ECS Best Practices**: см. `Doc/LTC-ECS Draft-191025-120316.md`
- **Логика игры и Request Flow**: см. `Doc/GAME_LOGIC.md`

---

## Контакты и поддержка

При возникновении вопросов обращайтесь к:
- Техническому заданию: `Doc/ТЗ_Inscryption_клон_с_балансом.md`
- Плану реализации: `.plan.md`
- ECS документации: `Doc/LTC-ECS Draft-191025-120316.md`

