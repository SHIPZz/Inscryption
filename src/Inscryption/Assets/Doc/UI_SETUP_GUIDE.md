# Инструкция по созданию UI для игры Inscryption

## Что уже готово (код):
✅ GameHUD.cs - MonoBehaviour компонент для UI
✅ UIComponents.cs - Entitas компоненты
✅ UpdateHealthUISystem.cs - обновление HP
✅ UpdateTurnIndicatorUISystem.cs - обновление индикатора хода
✅ ProcessEndTurnButtonSystem.cs - обработка кнопки завершения хода
✅ UIFeature.cs - интеграция в систему

## Что нужно сделать в Unity Editor:

### Шаг 1: Создать Canvas

1. В иерархии сцены: ПКМ → UI → Canvas
2. Назвать: **GameCanvas**
3. Canvas Scaler:
   - UI Scale Mode: **Scale With Screen Size**
   - Reference Resolution: **1920 x 1080**
   - Match: **0.5** (середина между Width и Height)

---

### Шаг 2: Создать GameObject для HUD

1. ПКМ на GameCanvas → Create Empty
2. Назвать: **GameHUD**
3. Добавить компонент **GameHUD** (скрипт)
4. RectTransform:
   - Anchors: **Stretch-Stretch** (все края)
   - Left/Right/Top/Bottom: **0**

---

### Шаг 3: Создать Health Display (Здоровье)

#### Hero Health (слева вверху):
1. ПКМ на GameHUD → UI → TextMeshPro - Text
2. Назвать: **HeroHealthText**
3. RectTransform:
   - Anchors: **Top-Left**
   - Pos X: **100**, Pos Y: **-50**
   - Width: **200**, Height: **60**
4. TextMeshPro:
   - Text: **"HP: 20/20"**
   - Font Size: **36**
   - Color: **Зеленый (#00FF00)**
   - Alignment: **Left + Middle**

#### Enemy Health (справа вверху):
1. ПКМ на GameHUD → UI → TextMeshPro - Text
2. Назвать: **EnemyHealthText**
3. RectTransform:
   - Anchors: **Top-Right**
   - Pos X: **-100**, Pos Y: **-50**
   - Width: **200**, Height: **60**
4. TextMeshPro:
   - Text: **"HP: 20/20"**
   - Font Size: **36**
   - Color: **Красный (#FF0000)**
   - Alignment: **Right + Middle**

---

### Шаг 4: Создать Turn Indicator (Индикатор хода)

#### Turn Indicator Text (по центру вверху):
1. ПКМ на GameHUD → UI → TextMeshPro - Text
2. Назвать: **TurnIndicatorText**
3. RectTransform:
   - Anchors: **Top-Center**
   - Pos X: **0**, Pos Y: **-50**
   - Width: **400**, Height: **80**
4. TextMeshPro:
   - Text: **"YOUR TURN"**
   - Font Size: **48**
   - Font Style: **Bold**
   - Color: **Желтый (#FFFF00)**
   - Alignment: **Center + Middle**

#### Hero Turn Indicator (визуальный маркер):
1. ПКМ на GameHUD → UI → Image
2. Назвать: **HeroTurnIndicator**
3. RectTransform:
   - Anchors: **Top-Left**
   - Pos X: **50**, Pos Y: **-120**
   - Width: **300**, Height: **10**
4. Image:
   - Color: **Зеленый (#00FF00)**
   - Raycast Target: **false**

#### Enemy Turn Indicator:
1. ПКМ на GameHUD → UI → Image
2. Назвать: **EnemyTurnIndicator**
3. RectTransform:
   - Anchors: **Top-Right**
   - Pos X: **-50**, Pos Y: **-120**
   - Width: **300**, Height: **10**
4. Image:
   - Color: **Красный (#FF0000)**
   - Raycast Target: **false**

---

### Шаг 5: Создать End Turn Button

1. ПКМ на GameHUD → UI → Button - TextMeshPro
2. Назвать: **EndTurnButton**
3. RectTransform:
   - Anchors: **Bottom-Center**
   - Pos X: **0**, Pos Y: **100**
   - Width: **300**, Height: **80**
4. Button:
   - Transition: **Color Tint**
   - Normal Color: **Белый**
   - Highlighted: **Светло-желтый**
   - Pressed: **Серый**
   - Disabled: **Темно-серый**
5. У дочернего **Text (TMP)**:
   - Text: **"END TURN"**
   - Font Size: **32**
   - Font Style: **Bold**
   - Color: **Черный**
   - Alignment: **Center + Middle**

---

### Шаг 6: Связать ссылки в GameHUD компоненте

1. Выбрать **GameHUD** GameObject
2. В Inspector найти компонент **GameHUD (Script)**
3. Перетащить объекты в соответствующие поля:
   - **Hero Health Text**: перетащить HeroHealthText
   - **Enemy Health Text**: перетащить EnemyHealthText
   - **Turn Indicator Text**: перетащить TurnIndicatorText
   - **Hero Turn Indicator**: перетащить HeroTurnIndicator
   - **Enemy Turn Indicator**: перетащить EnemyTurnIndicator
   - **End Turn Button**: перетащить EndTurnButton

---

### Шаг 7: Зарегистрировать GameHUD в системе

Создать скрипт инициализации или добавить в существующий EntryPoint:

```csharp
// В InitializeGameSystem.cs или отдельной системе инициализации UI
private void InitializeUI()
{
    GameHUD hud = GameObject.FindObjectOfType<GameHUD>();
    if (hud != null)
    {
        _meta.ReplaceGameHUD(hud);
        Debug.Log("[UI] GameHUD registered in MetaContext");
    }
    else
    {
        Debug.LogError("[UI] GameHUD not found on scene!");
    }
}
```

---

## Результат:

После выполнения всех шагов UI будет:
- ✅ Показывать здоровье игрока (зеленый текст слева)
- ✅ Показывать здоровье врага (красный текст справа)
- ✅ Индикатор текущего хода по центру ("YOUR TURN" / "ENEMY TURN")
- ✅ Визуальные полоски-индикаторы чья сейчас очередь
- ✅ Кнопка "END TURN" активна только в ход игрока
- ✅ Автоматическое обновление через Entitas системы

---

## Опциональные улучшения:

### Анимации:
- Добавить DOTween анимации для появления текста хода
- Пульсация индикаторов хода
- Плавное изменение цвета здоровья при уроне

### Звуки:
- Звук переключения хода
- Звук клика кнопки
- Звук получения урона

### Дополнительная информация:
- Счетчик карт в руке
- Счетчик карт в колоде
- Таймер хода
- История действий
