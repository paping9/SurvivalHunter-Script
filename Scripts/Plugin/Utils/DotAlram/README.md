# DotAlram System (Tree Structure)

게임 내 계층적 알림 배지 시스템입니다. Tree 구조를 지원하여 자식 노드의 알림이 부모 노드까지 자동으로 전파됩니다.

## 개요

DotAlram은 UI 요소에 표시되는 알림 배지(dot notification)를 Tree 구조로 관리하는 시스템입니다.
- **Tree 구조**: 부모-자식 관계를 가진 계층적 알림 시스템
- **Bubble-up**: 자식 노드에 알림이 추가되면 부모 노드까지 자동으로 카운트가 전파
- **제네릭 설계**: 알림 타입은 `int` 기반으로 완전히 재사용 가능
- **SignalHub 연동**: 알림 업데이트 시 자동으로 이벤트 발생

## 아키텍처

```
Plugin (Core)
├─ DotAlramController      ← 모든 알림을 flat하게 관리
├─ DotAlramElement          ← Tree 구조를 가진 알림 요소
└─ UpdateDotAlramMessage    ← 알림 업데이트 Signal

Logic (GameLogic)
└─ Tree 구조 생성 및 DotAlramElement 조립
   └─ Controller에 등록
```

**핵심 개념:**
- **DotAlramController**: 모든 알림 요소를 flat Dictionary로 관리
- **DotAlramElement**: Parent/Children을 가진 Tree 구조
- **Logic 레이어**: Tree를 구성하고 Controller에 등록

## 클래스 구조

### AlramLevel (enum)
```csharp
public enum AlramLevel
{
    Low,      // 낮은 우선순위
    Middle,   // 중간 우선순위
    High      // 높은 우선순위
}
```

### DotAlramElement
```csharp
public class DotAlramElement
{
    public int                      TypeId      { get; }  // 알림 타입 ID
    public AlramLevel               AlramLevel  { get; }  // 우선순위
    public int                      Count       { get; }  // 알림 개수
    public int                      Value       { get; }  // 서브 카테고리

    // Tree 구조
    public DotAlramElement          Parent      { get; }
    public List<DotAlramElement>    Children    { get; }

    // Tree 탐색
    bool IsRoot                                           // 루트 노드인지
    bool IsLeaf                                           // 말단 노드인지
    DotAlramElement FindChild(int typeId, int value)      // 직접 자식 찾기
    DotAlramElement FindDescendant(int typeId, int value) // 재귀적으로 자손 찾기
    DotAlramElement GetRoot()                             // 루트 노드 가져오기

    // Count 관리
    void UpdateCount(int count)         // Count 업데이트 (부모에게 자동 전파)
    void RecalculateCount()             // 자식들의 Count 합산
}
```

### DotAlramController
```csharp
public class DotAlramController
{
    // 등록/조회
    void RegisterElement(DotAlramElement element)
    DotAlramElement GetElement(int typeId, int value = 0)
    IEnumerable<DotAlramElement> GetAllElements()

    // Count 조작
    void AddAlram(DotAlramElement element, int count = 1)
    void RemoveAlram(DotAlramElement element, int count = 1)
    void ClearAlram(DotAlramElement element)

    // 제거
    void RemoveElement(int typeId, int value = 0)
    void Clear()
}
```

## 사용 예시

### 예시 1: Event Root 트리 구조

```
EventRoot (TypeId: 100)
├─ EventGroup (TypeId: 101)
│   ├─ EventQuest (TypeId: 102)
│   │   ├─ Quest_1 (TypeId: 102, Value: 1)  ← 실제 퀘스트
│   │   ├─ Quest_2 (TypeId: 102, Value: 2)
│   │   └─ Quest_3 (TypeId: 102, Value: 3)
│   └─ EventShop (TypeId: 103)
│       ├─ Shop_Item_1 (TypeId: 103, Value: 1)
│       └─ Shop_Item_2 (TypeId: 103, Value: 2)
└─ SeasonalEvent (TypeId: 104)
    └─ ...
```

**동작 방식:**
1. `Quest_1`에 알림 추가 (Count: 1)
2. `EventQuest` 자동 갱신 (Count: 1)
3. `EventGroup` 자동 갱신 (Count: 1)
4. `EventRoot` 자동 갱신 (Count: 1)

### Logic 레이어에서 Tree 생성

```csharp
using DotAlram;
using Defs;
using Message;
using VContainer;

public class EventAlramManager
{
    private DotAlramController  _dotAlramController;
    private ISignalHub          _signalHub;

    // Tree 구조 저장
    private DotAlramElement _eventRoot;
    private DotAlramElement _eventGroup;
    private DotAlramElement _eventQuest;
    private DotAlramElement _eventShop;

    [Inject]
    public void Construct(DotAlramController controller, ISignalHub signalHub)
    {
        _dotAlramController = controller;
        _signalHub          = signalHub;

        InitializeEventTree();
    }

    /// <summary>
    /// Event 알림 트리 초기화
    /// </summary>
    private void InitializeEventTree()
    {
        // 1. Root 생성
        _eventRoot = new DotAlramElement(
            (int)DotAlramType.EventRoot,
            AlramLevel.High,
            0,
            0,
            _signalHub,
            parent: null  // Root는 parent 없음
        );
        _dotAlramController.RegisterElement(_eventRoot);

        // 2. EventGroup 생성 (EventRoot의 자식)
        _eventGroup = new DotAlramElement(
            (int)DotAlramType.EventGroup,
            AlramLevel.High,
            0,
            0,
            _signalHub,
            parent: _eventRoot  // ← 부모 지정
        );
        _dotAlramController.RegisterElement(_eventGroup);

        // 3. EventQuest 생성 (EventGroup의 자식)
        _eventQuest = new DotAlramElement(
            (int)DotAlramType.EventQuest,
            AlramLevel.Middle,
            0,
            0,
            _signalHub,
            parent: _eventGroup  // ← 부모 지정
        );
        _dotAlramController.RegisterElement(_eventQuest);

        // 4. EventShop 생성 (EventGroup의 자식)
        _eventShop = new DotAlramElement(
            (int)DotAlramType.EventShop,
            AlramLevel.Middle,
            0,
            0,
            _signalHub,
            parent: _eventGroup  // ← 부모 지정
        );
        _dotAlramController.RegisterElement(_eventShop);
    }

    /// <summary>
    /// 특정 퀘스트에 알림 추가
    /// </summary>
    public void AddQuestAlram(int questId)
    {
        // 1. 해당 Quest의 알림 요소 찾기
        var questElement = _eventQuest.FindChild((int)DotAlramType.EventQuest, questId);

        // 2. 없으면 새로 생성
        if (questElement == null)
        {
            questElement = new DotAlramElement(
                (int)DotAlramType.EventQuest,
                AlramLevel.Low,
                0,
                questId,  // ← Value로 questId 사용
                _signalHub,
                parent: _eventQuest  // ← EventQuest를 부모로
            );
            _dotAlramController.RegisterElement(questElement);
        }

        // 3. 알림 추가 (부모 노드까지 자동 전파)
        _dotAlramController.AddAlram(questElement, 1);

        // 결과:
        // - questElement Count: 1
        // - _eventQuest Count: 1 (자식 합산)
        // - _eventGroup Count: 1 (자식 합산)
        // - _eventRoot Count: 1 (자식 합산)
    }

    /// <summary>
    /// 특정 퀘스트 알림 제거
    /// </summary>
    public void RemoveQuestAlram(int questId)
    {
        var questElement = _eventQuest.FindChild((int)DotAlramType.EventQuest, questId);

        if (questElement != null)
        {
            _dotAlramController.ClearAlram(questElement);
            // 부모 노드들도 자동으로 재계산됨
        }
    }

    /// <summary>
    /// Shop 아이템에 알림 추가
    /// </summary>
    public void AddShopItemAlram(int itemId)
    {
        var shopItemElement = _eventShop.FindChild((int)DotAlramType.EventShop, itemId);

        if (shopItemElement == null)
        {
            shopItemElement = new DotAlramElement(
                (int)DotAlramType.EventShop,
                AlramLevel.Low,
                0,
                itemId,
                _signalHub,
                parent: _eventShop
            );
            _dotAlramController.RegisterElement(shopItemElement);
        }

        _dotAlramController.AddAlram(shopItemElement, 1);

        // 결과:
        // - shopItemElement Count: 1
        // - _eventShop Count: 1
        // - _eventGroup Count: 2 (EventQuest + EventShop)
        // - _eventRoot Count: 2
    }
}
```

### Logic 레이어에서 enum 정의

```csharp
// Logic/Defs/Enum/DotAlramEnum.cs
namespace Defs
{
    public enum DotAlramType
    {
        // Event 관련
        EventRoot       = 100,
        EventGroup      = 101,
        EventQuest      = 102,
        EventShop       = 103,
        SeasonalEvent   = 104,

        // Skill 관련
        SkillRoot       = 200,
        SkillTree       = 201,
        NewSkill        = 202,

        // Inventory 관련
        InventoryRoot   = 300,
        NewItem         = 301,
        // ...
    }
}
```

### UI에서 알림 리스닝

```csharp
public class EventUIHandler
{
    private ISignalHub _signalHub;

    [Inject]
    public void Construct(ISignalHub signalHub)
    {
        _signalHub = signalHub;

        // 알림 업데이트 리스닝
        _signalHub.Get<UpdateDotAlramMessage>().AddListener(OnAlramUpdated);
    }

    private void OnAlramUpdated(int typeId, int count, int value)
    {
        var type = (DotAlramType)typeId;

        switch (type)
        {
            case DotAlramType.EventRoot:
                UpdateEventRootBadge(count);
                break;

            case DotAlramType.EventGroup:
                UpdateEventGroupBadge(count);
                break;

            case DotAlramType.EventQuest:
                UpdateEventQuestBadge(count, value);
                break;

            case DotAlramType.EventShop:
                UpdateEventShopBadge(count, value);
                break;
        }
    }

    private void UpdateEventRootBadge(int count)
    {
        // EventRoot UI 배지 업데이트
        _eventRootBadge.SetActive(count > 0);
        _eventRootBadgeText.text = count.ToString();
    }

    private void UpdateEventQuestBadge(int count, int questId)
    {
        if (questId == 0)
        {
            // EventQuest 그룹 전체 배지
            _eventQuestGroupBadge.SetActive(count > 0);
            _eventQuestGroupBadgeText.text = count.ToString();
        }
        else
        {
            // 특정 Quest 배지
            var questBadge = GetQuestBadge(questId);
            questBadge.SetActive(count > 0);
        }
    }
}
```

## 고급 사용법

### 동적으로 Leaf 노드 추가

```csharp
public void OnNewQuestAvailable(int questId)
{
    // EventQuest 노드에 새로운 퀘스트 추가
    var newQuestElement = new DotAlramElement(
        (int)DotAlramType.EventQuest,
        AlramLevel.Low,
        1,  // 바로 알림 1개
        questId,
        _signalHub,
        parent: _eventQuest  // 기존 EventQuest에 연결
    );

    _dotAlramController.RegisterElement(newQuestElement);
    // _eventQuest, _eventGroup, _eventRoot 모두 자동으로 Count 증가
}
```

### 특정 브랜치 전체 제거

```csharp
public void ClearAllQuestAlrams()
{
    // EventQuest의 모든 자식 제거
    foreach (var child in _eventQuest.Children.ToList())
    {
        _dotAlramController.ClearAlram(child);
        _dotAlramController.RemoveElement(child.TypeId, child.Value);
    }

    // 부모 노드들은 자동으로 재계산됨
}
```

### Root에서 특정 Descendant 찾기

```csharp
public void FindAndUpdateQuest(int questId)
{
    // EventRoot에서 재귀적으로 퀘스트 찾기
    var questElement = _eventRoot.FindDescendant(
        (int)DotAlramType.EventQuest,
        questId
    );

    if (questElement != null)
    {
        _dotAlramController.AddAlram(questElement, 1);
    }
}
```

## DI 등록 (GameDiContainer.cs)

```csharp
builder.Register<DotAlramController>(Lifetime.Singleton);
```

## 주요 특징

| 기능 | 설명 |
|------|------|
| **Bubble-up** | 자식 노드 변경 시 부모 노드까지 자동 전파 |
| **Auto-recalculation** | 자식들의 Count를 자동으로 합산 |
| **Tree 탐색** | FindChild, FindDescendant로 쉬운 노드 검색 |
| **Flat 관리** | Controller는 모든 노드를 flat Dictionary로 관리 |
| **재사용성** | Plugin 코드는 게임 로직과 완전히 분리 |

## 동작 흐름

```
1. Logic에서 Tree 구조 생성
   EventRoot → EventGroup → EventQuest
                          → EventShop

2. 각 노드를 Controller에 등록
   controller.RegisterElement(eventRoot)
   controller.RegisterElement(eventGroup)
   ...

3. Leaf 노드에 알림 추가
   controller.AddAlram(questElement, 1)

4. 자동 Bubble-up
   questElement.UpdateCount(1)
   → eventQuest.RecalculateCount()
   → eventGroup.RecalculateCount()
   → eventRoot.RecalculateCount()

5. 각 단계마다 Signal 발생
   UpdateDotAlramMessage.Dispatch(typeId, count, value)

6. UI에서 Signal 수신 및 배지 업데이트
```

## 주의사항

1. **Tree 구성**: Tree는 반드시 Logic 레이어에서 생성
2. **등록 필수**: 모든 노드는 Controller에 등록해야 함
3. **Parent 지정**: 생성자에서 parent 파라미터로 부모 지정
4. **Value 활용**: 같은 TypeId에서 여러 인스턴스는 Value로 구분
5. **메모리 관리**: 불필요한 노드는 RemoveElement()로 제거

## 장점

- ✅ **계층적 알림**: 복잡한 메뉴 구조의 알림을 쉽게 관리
- ✅ **자동 전파**: 자식 변경 시 부모까지 자동 갱신
- ✅ **유연성**: 동적으로 노드 추가/제거 가능
- ✅ **재사용성**: Plugin 코드는 게임 독립적
- ✅ **타입 안정성**: Logic에서 enum 사용
