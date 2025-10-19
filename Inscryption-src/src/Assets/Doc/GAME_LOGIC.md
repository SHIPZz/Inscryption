# Логика игры Inscryption Clone

## Обзор архитектуры

Игра построена на **Request-Driven архитектуре**, где системы обрабатывают запросы (Request компоненты) и создают новые запросы для следующих систем. Это создает **цепочку обработки событий**.

---

## Структура Features

### GameplayRootFeature
**Главная фича**, которая объединяет все игровые фичи в правильном порядке:

```csharp
GameplayRootFeature
├── GameFeature        (Initialize) - инициализация и проверка победы
├── CardFeature        (Execute)    - обработка взятия карт
├── HeroFeature        (Execute)    - системы героя (пока пусто)
├── EnemyFeature       (Execute)    - AI врага
├── BoardFeature       (Execute)    - размещение карт на поле
├── TurnFeature        (Execute)    - управление ходами
└── BattleFeature      (Execute)    - боевые системы
```

**Порядок важен!** Системы выполняются в том порядке, в котором добавлены фичи.

---

## Детальное описание фич и систем

### 1. GameFeature - Инициализация и условия победы

#### InitializeGameSystem (IInitializeSystem)
**Когда:** Выполняется один раз при старте игры

**Что делает:**
1. Создает героя (HP=20, HeroTurn=true)
2. Создает врага (HP=20)
3. Создает игровое поле (8 слотов: 4 героя + 4 врага)
4. Создает колоду (30 карт: 15 героя + 15 врага)
5. Раздает стартовые руки (по 3 карты)

**Откуда вызывается:**
- `GameplayRootFeature.Initialize()` → запускается из `GameTestRunner.Start()`

**Создает сущности:**
- 1 Hero entity с компонентами: `Id`, `Hero`, `Hp`, `MaxHp`, `HeroTurn`, `CardsInHand`
- 1 Enemy entity с компонентами: `Id`, `Enemy`, `Hp`, `MaxHp`, `CardsInHand`
- 8 BoardSlot entities с компонентами: `Id`, `BoardSlot`, `SlotLane`, `SlotOwner`, `OccupiedBy`
- 30 Card entities с компонентами: `Id`, `Card`, `Hp`, `MaxHp`, `Damage`, `CardOwner`, `InHand`

---

#### CheckVictorySystem (IExecuteSystem)
**Когда:** Выполняется каждый кадр

**Что делает:**
- Проверяет HP героя и врага
- Если HP ≤ 0 → объявляет победителя и помечает проигравшего как `Destructed`

**Создает компоненты:**
- `Destructed` на проигравшем игроке

---

### 2. CardFeature - Взятие карт

#### ProcessDrawCardRequestSystem (IExecuteSystem)
**Когда:** Выполняется каждый кадр

**Обрабатывает Request:** `DrawCardRequest(playerId)`

**Что делает:**
1. Находит игрока по `playerId`
2. Проверяет лимит руки (MAX = 5 карт)
3. Находит доступную карту игрока из колоды
4. Добавляет карту в руку игрока (`CardsInHand.Add(cardId)`)
5. Удаляет `DrawCardRequest`

**Откуда создается DrawCardRequest:**
- `ProcessSwitchTurnRequestSystem` → после переключения хода создает `DrawCardRequest` для следующего игрока

---

### 3. HeroFeature - Системы героя
**Пусто** - зарезервировано для будущих систем (например, способности героя)

---

### 4. EnemyFeature - Искусственный интеллект

#### EnemyAISystem (IExecuteSystem)
**Когда:** Выполняется каждый кадр, пока ход врага (`EnemyTurn=true`)

**Проверки перед выполнением:**
- Если уже есть `EndTurnRequest` → пропустить
- Если уже есть `PlaceCardRequest` → пропустить

**Что делает:**
1. Проверяет наличие карт в руке
2. Проверяет, не размещена ли уже карта в этот ход (`CardsPlacedThisTurn`)
3. Если нет карт/слотов или карта уже размещена → создает `EndTurnRequest`
4. Если есть карты и слоты → выбирает случайную карту и слот
5. Создает `PlaceCardRequest(cardId, slotId)`

**Создает компоненты:**
- `PlaceCardRequest(cardId, slotId)` - для размещения карты
- `EndTurnRequest` - для завершения хода

**Оптимизации:**
- Использует `ListPool<GameEntity>` для списка доступных слотов (без аллокаций)

---

### 5. BoardFeature - Размещение карт

#### ProcessPlaceCardRequestSystem (IExecuteSystem)
**Когда:** Выполняется каждый кадр

**Обрабатывает Request:** `PlaceCardRequest(cardId, slotId)`

**Что делает:**
1. Находит карту и слот по ID
2. **Валидация:**
   - Карта в руке (`InHand=true`)
   - Слот является BoardSlot
   - Слот пустой (`OccupiedBy=-1`)
   - Слот принадлежит владельцу карты
   - Игрок не превысил лимит размещения (MAX = 1 карта/ход)
3. Если валидация успешна:
   - Снимает `InHand`, добавляет `OnBoard` и `Lane` на карту
   - Обновляет `OccupiedBy` на слоте
   - Удаляет карту из `CardsInHand` владельца
   - Увеличивает `CardsPlacedThisTurn` владельца
4. Удаляет `PlaceCardRequest`

**Откуда создается PlaceCardRequest:**
- `EnemyAISystem` → для AI врага
- `GameTestRunner.TryPlaceHeroCard()` → для игрока (по нажатию клавиш 1-4)

---

### 6. TurnFeature - Управление ходами

#### ProcessEndTurnRequestSystem (IExecuteSystem)
**Когда:** Выполняется каждый кадр

**Обрабатывает Request:** `EndTurnRequest`

**Проверки перед выполнением:**
- Если уже есть `AttackPhase` → удалить все `EndTurnRequest` и пропустить

**Что делает:**
1. Находит активного игрока (у кого `HeroTurn=true` или `EnemyTurn=true`)
2. Создает маркер `AttackPhase`
3. Удаляет **ВСЕ** `EndTurnRequest`

**Создает компоненты:**
- `AttackPhase` (маркер фазы атаки)

**Откуда создается EndTurnRequest:**
- `EnemyAISystem` → когда AI завершает свой ход
- `GameTestRunner.Update()` → когда игрок нажимает SPACE

---

#### ProcessSwitchTurnRequestSystem (IExecuteSystem)
**Когда:** Выполняется каждый кадр

**Обрабатывает Request:** `SwitchTurnRequest`

**Что делает:**
1. Определяет текущего игрока (у кого ход)
2. **Переключение хода:**
   - Снимает `HeroTurn` / `EnemyTurn` с текущего
   - Добавляет `HeroTurn` / `EnemyTurn` следующему
   - Сбрасывает `CardsPlacedThisTurn` обоим игрокам
3. Создает `DrawCardRequest` для следующего игрока
4. Удаляет **ВСЕ** `SwitchTurnRequest`

**Флаг `turnSwitched`:**
- Гарантирует, что переключение произойдет только один раз, даже если request'ов несколько

**Создает компоненты:**
- `DrawCardRequest(nextPlayerId)` - для взятия карты

**Откуда создается SwitchTurnRequest:**
- `ProcessAttackPhaseSystem` → после обработки всех атак

---

### 7. BattleFeature - Боевые системы

#### ProcessAttackPhaseSystem (IExecuteSystem)
**Когда:** Выполняется каждый кадр

**Обрабатывает маркер:** `AttackPhase`

**Проверки перед выполнением:**
- Если уже есть `SwitchTurnRequest` → удалить все `AttackPhase` и пропустить

**Что делает:**
1. Находит активного игрока
2. Собирает все карты активного игрока на поле
3. Для каждой карты:
   - Ищет вражескую карту на той же линии (`Lane`)
   - Если нашел → создает `AttackRequest(attackerId=card, targetId=enemyCard, damage)`
   - Если не нашел → создает `AttackRequest(attackerId=card, targetId=opponent, damage)`
4. Создает `SwitchTurnRequest`
5. Удаляет **ВСЕ** `AttackPhase`

**Создает компоненты:**
- `AttackRequest(attackerId, targetId, damage)` - для каждой атакующей карты
- `SwitchTurnRequest` - для переключения хода

**Откуда создается AttackPhase:**
- `ProcessEndTurnRequestSystem` → при завершении хода

---

#### ProcessAttackRequestSystem (IExecuteSystem)
**Когда:** Выполняется каждый кадр

**Обрабатывает Request:** `AttackRequest(attackerId, targetId, damage)`

**Что делает:**
1. Находит атакующего и цель по ID
2. Применяет урон: `target.Hp -= damage`
3. Если `target.Hp ≤ 0`:
   - Помечает цель как `Destructed`
   - Если цель - карта на поле → освобождает слот (`OccupiedBy=-1`)
4. Удаляет `AttackRequest`

**Создает компоненты:**
- `Destructed` на уничтоженных картах/игроках

**Откуда создается AttackRequest:**
- `ProcessAttackPhaseSystem` → для каждой атакующей карты

---

## Полный цикл игры (Flow)

### Инициализация (один раз)
```
GameTestRunner.Start()
  └─> GameplayRootFeature.Initialize()
      └─> InitializeGameSystem.Initialize()
          ├─> HeroFactory.CreateHero(20)        [Hero, HeroTurn=true]
          ├─> EnemyFactory.CreateEnemy(20)      [Enemy]
          ├─> BoardFactory.CreateSlots(...)     [8 BoardSlots]
          ├─> CardFactory.CreateRandomCard() x30 [30 Cards, InHand=true]
          └─> Раздача 3 карт в руку (CardsInHand)
```

---

### Ход героя (повторяется)

```
[Игрок нажимает 1-4]
  └─> GameTestRunner.TryPlaceHeroCard(laneIndex)
      └─> CREATE PlaceCardRequest(cardId, slotId)

[Каждый кадр]
  └─> ProcessPlaceCardRequestSystem.Execute()
      └─> PROCESS PlaceCardRequest
          ├─> Валидация
          ├─> card.InHand=false, card.OnBoard=true
          ├─> slot.OccupiedBy=cardId
          ├─> owner.CardsInHand.Remove(cardId)
          ├─> owner.CardsPlacedThisTurn++
          └─> DELETE PlaceCardRequest

[Игрок нажимает SPACE]
  └─> GameTestRunner.Update()
      └─> CREATE EndTurnRequest

[Каждый кадр]
  └─> ProcessEndTurnRequestSystem.Execute()
      └─> PROCESS EndTurnRequest
          ├─> CREATE AttackPhase
          └─> DELETE ALL EndTurnRequest

[Каждый кадр]
  └─> ProcessAttackPhaseSystem.Execute()
      └─> PROCESS AttackPhase
          ├─> Для каждой карты героя на поле:
          │   └─> CREATE AttackRequest(card, target, damage)
          ├─> CREATE SwitchTurnRequest
          └─> DELETE ALL AttackPhase

[Каждый кадр]
  └─> ProcessAttackRequestSystem.Execute()
      └─> PROCESS AttackRequest
          ├─> target.Hp -= damage
          ├─> Если target.Hp ≤ 0 → target.Destructed=true
          └─> DELETE AttackRequest

[Каждый кадр]
  └─> ProcessSwitchTurnRequestSystem.Execute()
      └─> PROCESS SwitchTurnRequest
          ├─> hero.HeroTurn=false, enemy.EnemyTurn=true
          ├─> Сброс CardsPlacedThisTurn
          ├─> CREATE DrawCardRequest(enemyId)
          └─> DELETE ALL SwitchTurnRequest

[Каждый кадр]
  └─> ProcessDrawCardRequestSystem.Execute()
      └─> PROCESS DrawCardRequest
          ├─> enemy.CardsInHand.Add(cardId)
          └─> DELETE DrawCardRequest
```

---

### Ход врага (повторяется)

```
[Каждый кадр, пока EnemyTurn=true]
  └─> EnemyAISystem.Execute()
      └─> IF нет PlaceCardRequest И нет EndTurnRequest:
          ├─> Выбрать случайную карту и слот
          └─> CREATE PlaceCardRequest(cardId, slotId)

[Каждый кадр]
  └─> ProcessPlaceCardRequestSystem.Execute()
      └─> (обработка как у героя)

[Через 2 секунды или при условиях AI]
  └─> EnemyAISystem.Execute()
      └─> CREATE EndTurnRequest

[Далее цикл как у героя:]
  ProcessEndTurnRequestSystem
  → ProcessAttackPhaseSystem
  → ProcessAttackRequestSystem
  → ProcessSwitchTurnRequestSystem
  → ProcessDrawCardRequestSystem
```

---

## Правила создания Request-компонентов

### 1. Проверка существования перед созданием
**Всегда проверяй**, что Request/Phase еще не существует:

```csharp
if (_endTurnRequests.count > 0)
    return; // Не создавать новый!
```

**Примеры:**
- `EnemyAISystem` → проверяет `EndTurnRequest` и `PlaceCardRequest`
- `ProcessEndTurnRequestSystem` → проверяет `AttackPhase`
- `ProcessAttackPhaseSystem` → проверяет `SwitchTurnRequest`

---

### 2. Обработка только первого, удаление всех
**Обрабатывай** только первый Request, но **удаляй ВСЕ**:

```csharp
bool processed = false;

foreach (GameEntity request in _requests.GetEntities(_buffer))
{
    if (!processed)
    {
        // Обработать логику
        processed = true;
    }
    
    request.isDestructed = true; // Удалить ВСЕ
}
```

---

### 3. Цепочка Request → Response
Каждая система создает Request для следующей системы:

```
EndTurnRequest 
  → (ProcessEndTurnRequestSystem) → 
AttackPhase 
  → (ProcessAttackPhaseSystem) → 
AttackRequest + SwitchTurnRequest
  → (ProcessAttackRequestSystem) → урон
  → (ProcessSwitchTurnRequestSystem) → 
DrawCardRequest
  → (ProcessDrawCardRequestSystem) → взятие карты
```

---

## Важные моменты архитектуры

### 1. Request-компоненты живут один кадр
- Создаются в кадре N
- Обрабатываются в кадре N (или N+1)
- Удаляются после обработки (`isDestructed=true`)

### 2. Маркеры (Phase) также одноразовые
- `AttackPhase` создается и удаляется за один цикл
- Предотвращает повторную обработку

### 3. Порядок систем имеет значение
```
CardFeature → EnemyFeature → BoardFeature → TurnFeature → BattleFeature
```
Если изменить порядок, логика сломается!

### 4. Cleanup системы
После всех Execute систем запускаются Cleanup системы:
- `CleanupGameDestructedSystem` → удаляет сущности с `Destructed=true`

### 5. Один Request = Одно действие
Не создавай несколько Request'ов для одного действия:
```csharp
// ❌ ПЛОХО
_game.CreateEntity().isEndTurnRequest = true;
_game.CreateEntity().isEndTurnRequest = true; // Дубликат!

// ✅ ХОРОШО
if (_endTurnRequests.count == 0)
    _game.CreateEntity().isEndTurnRequest = true;
```

---

## Диаграмма зависимостей Request'ов

```
[User Input: SPACE] ──────────────┐
[EnemyAI: логика]  ────────────┐  │
                               │  │
                               ↓  ↓
                         EndTurnRequest
                               │
                ProcessEndTurnRequestSystem
                               │
                               ↓
                          AttackPhase
                               │
                 ProcessAttackPhaseSystem
                               │
                    ┌──────────┴──────────┐
                    ↓                     ↓
            AttackRequest          SwitchTurnRequest
                    │                     │
     ProcessAttackRequestSystem  ProcessSwitchTurnRequestSystem
                    │                     │
                    ↓                     ↓
        [Урон, Destructed]          DrawCardRequest
                                          │
                              ProcessDrawCardRequestSystem
                                          │
                                          ↓
                                [Карта в руке]
```

---

## Отладка и логирование

Каждая система логирует свои действия:
```
[ProcessEndTurnRequestSystem] End turn for player X, starting attack phase
[ProcessAttackPhaseSystem] Processing attacks for player X
[ProcessAttackPhaseSystem] Created attack: Card Y -> Target Z (Damage: D)
[ProcessAttackRequestSystem] Attack: Y -> Z, Damage: D, HP: 10 -> 5
[ProcessSwitchTurnRequestSystem] Turn switched to Enemy (ID=2)
[ProcessDrawCardRequestSystem] Player 2 drew card 15 (HP=3, DMG=2)
```

**Для отладки бесконечных циклов:**
- Проверяй количество одинаковых логов подряд
- Если одна система логирует тысячи раз → Request не удаляется или создается снова

---

## Примеры расширения

### Добавление новой механики: "Buff Cards"

1. **Компонент:**
```csharp
[Game] public class BuffRequest : IComponent { 
    public int CardId; 
    public int BuffValue; 
}
```

2. **Система:**
```csharp
public class ProcessBuffRequestSystem : IExecuteSystem
{
    public void Execute()
    {
        foreach (GameEntity request in _buffRequests.GetEntities(_buffer))
        {
            GameEntity card = _game.GetEntityWithId(request.buffRequest.CardId);
            card.ReplaceDamage(card.Damage + request.buffRequest.BuffValue);
            
            request.isDestructed = true;
        }
    }
}
```

3. **Добавить в Feature:**
```csharp
public class CardFeature : Feature
{
    public CardFeature(ISystemFactory systemFactory)
    {
        Add(systemFactory.Create<ProcessDrawCardRequestSystem>());
        Add(systemFactory.Create<ProcessBuffRequestSystem>()); // ← Новая
    }
}
```

4. **Создать Request:**
```csharp
// Где-то в логике
_game.CreateEntity().AddBuffRequest(cardId, buffValue: 2);
```

---

## Заключение

**Request-Driven архитектура** позволяет:
✅ Ясно видеть цепочку событий  
✅ Легко добавлять новые механики  
✅ Тестировать системы независимо  
✅ Избегать спагетти-кода  
✅ Контролировать порядок выполнения  

**Главное правило:** Каждая система должна быть **идемпотентной** - повторный вызов с теми же Request'ами не должен вызывать проблем (благодаря проверкам существования и флагам).

