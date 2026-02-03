# Wave-Based Loading System Changes

## Overview

The enemy system has been refactored to load enemy models and data separately for each wave, using wave-number-based naming conventions. This provides better flexibility and allows unique enemies for each wave.

---

## What Changed

### 1. Asset Naming Convention

**OLD System:**
- Enemy models: `Ground.prefab`, `Flying.prefab` (based on EnemyMoveType)
- Enemy data: Custom names like `GroundEnemy_Wave1.asset`, `FlyingEnemy_Wave5.asset`

**NEW System:**
- Enemy models: `Enemy_1.prefab`, `Enemy_2.prefab`, `Enemy_3.prefab`, etc. (based on wave number)
- Enemy data: `EnemyData_1.asset`, `EnemyData_2.asset`, `EnemyData_3.asset`, etc. (based on wave number)

### 2. WaveConfigData Structure

**File:** `Assets/Scripts/infrastructure/services/waveService/WaveConfigData.cs`

**OLD:**
```csharp
public class WaveConfigData : ScriptableObject
{
    public int WaveNumber;
    public int EnemyCount;
    public float SpawnInterval;
    public EnemyData EnemyData;  // Direct reference
}
```

**NEW:**
```csharp
public class WaveConfigData : ScriptableObject
{
    public int WaveNumber;
    public int EnemyCount;
    public float SpawnInterval;
    // EnemyData is now loaded dynamically by wave number
}
```

**Impact:** Wave configs no longer need to reference EnemyData assets. The system loads them automatically by wave number.

### 3. IEnemyFactory Interface

**File:** `Assets/Scripts/infrastructure/factories/enemies/IEnemyFactory.cs`

**OLD:**
```csharp
Enemy CreateEnemy(EnemyData enemyData, Vector3 position);
```

**NEW:**
```csharp
Enemy CreateEnemy(EnemyData enemyData, Vector3 position, int waveNumber);
```

**Impact:** Factory now requires wave number to load the correct visual model.

### 4. EnemyFactory Implementation

**File:** `Assets/Scripts/infrastructure/factories/enemies/EnemyFactory.cs`

**Key Changes:**

**Model Loading (OLD):**
- Loaded `Ground.prefab` and `Flying.prefab` in constructor
- Selected model based on `EnemyData.EnemyMoveType`

**Model Loading (NEW):**
- Models loaded on-demand by wave number
- Path format: `"Prefabs/Enemies/Models/Enemy_{waveNumber}"`
- Cached in dictionary for performance: `Dictionary<int, GameObject> _enemyModels`

**Example:**
```csharp
private GameObject LoadEnemyModel(int waveNumber)
{
    // Check cache first
    if (_enemyModels.ContainsKey(waveNumber))
        return _enemyModels[waveNumber];

    // Load model for this wave
    string path = $"Prefabs/Enemies/Models/Enemy_{waveNumber}";
    GameObject model = _resourceProvider.Load<GameObject>(path);

    if (model != null)
        _enemyModels[waveNumber] = model;

    return model;
}
```

### 5. WaveService Implementation

**File:** `Assets/Scripts/infrastructure/services/waveService/WaveService.cs`

**New Method: LoadEnemyData()**
```csharp
private EnemyData LoadEnemyData(int waveNumber)
{
    // Load enemy data specific to this wave number
    string path = $"ScriptableObjects/Enemies/EnemyData_{waveNumber}";
    EnemyData enemyData = _resourceProvider.Load<EnemyData>(path);

    if (enemyData == null)
    {
        Debug.LogWarning($"Enemy data not found at {path}, using fallback");
        enemyData = CreateFallbackEnemyData(waveNumber);
    }

    return enemyData;
}
```

**New Method: CreateFallbackEnemyData()**
```csharp
private EnemyData CreateFallbackEnemyData(int waveNumber)
{
    var enemyData = ScriptableObject.CreateInstance<EnemyData>();

    // Scale stats based on wave number
    enemyData.Health = 100 + (waveNumber - 1) * 20;
    enemyData.MoveSpeed = 2 + (waveNumber / 10f);
    enemyData.RotateSpeed = 180;
    enemyData.Damage = 1 + (waveNumber / 5);
    enemyData.Armor = waveNumber / 3;
    enemyData.MagicResist = waveNumber / 3;
    enemyData.Evasion = 0;

    // Every 5th wave is flying
    enemyData.EnemyMoveType = (waveNumber % 5 == 0) ? EnemyMoveType.Flying : EnemyMoveType.Ground;

    return enemyData;
}
```

**Updated StartWave() Logic:**
```csharp
// Load wave configuration
WaveConfigData config = LoadWaveConfig(_currentWaveNumber);

// Load enemy data for this wave (NEW)
EnemyData enemyData = LoadEnemyData(_currentWaveNumber);

// Spawn enemies with wave number (NEW)
Enemy enemy = _enemyFactory.CreateEnemy(enemyData, spawnPosition, _currentWaveNumber);
```

---

## Benefits of New System

### 1. **Unique Visuals Per Wave**
Each wave can have completely unique enemy appearance without being limited to "Ground" or "Flying" models.

Examples:
- Wave 1: Small red slime
- Wave 2: Medium blue slime
- Wave 5: Flying bat
- Wave 10: Large dragon
- Wave 15: Armored knight

### 2. **Simplified Asset Management**
Assets are organized by wave number, making it clear which assets correspond to which waves.

**Asset Structure:**
```
Resources/
├── Prefabs/Enemies/Models/
│   ├── Enemy_1.prefab   (Wave 1 visual)
│   ├── Enemy_2.prefab   (Wave 2 visual)
│   ├── Enemy_5.prefab   (Wave 5 visual)
│   └── ...
└── ScriptableObjects/Enemies/
    ├── EnemyData_1.asset  (Wave 1 stats)
    ├── EnemyData_2.asset  (Wave 2 stats)
    ├── EnemyData_5.asset  (Wave 5 stats)
    └── ...
```

### 3. **Automatic Fallback System**
If assets are missing for a wave, the system automatically generates:
- Procedural enemy stats (health, speed, damage scale with wave number)
- Flying enemies every 5th wave
- Progressive difficulty increase

This means the game works even with minimal asset creation.

### 4. **No WaveConfigData Changes Needed**
When adding new enemy types, you only need to create:
- `EnemyData_{N}.asset` (stats)
- `Enemy_{N}.prefab` (visual)

No need to update WaveConfigData or create cross-references.

---

## Asset Creation Guide

### Minimum Setup (3 Waves)

**Create these files:**

1. **Enemy Models** (Visual prefabs with no scripts):
   - `Resources/Prefabs/Enemies/Models/Enemy_1.prefab`
   - `Resources/Prefabs/Enemies/Models/Enemy_2.prefab`
   - `Resources/Prefabs/Enemies/Models/Enemy_5.prefab`

2. **Enemy Data** (ScriptableObject with stats):
   - `Resources/ScriptableObjects/Enemies/EnemyData_1.asset`
   - `Resources/ScriptableObjects/Enemies/EnemyData_2.asset`
   - `Resources/ScriptableObjects/Enemies/EnemyData_5.asset`

3. **Wave Configs** (ScriptableObject with count/interval):
   - `Resources/ScriptableObjects/Waves/Wave_01.asset`
   - `Resources/ScriptableObjects/Waves/Wave_02.asset`
   - `Resources/ScriptableObjects/Waves/Wave_05.asset`

### Full Setup (20 Waves)

Create the same pattern for waves 1-20:
- 20 enemy models: `Enemy_1.prefab` through `Enemy_20.prefab`
- 20 enemy data assets: `EnemyData_1.asset` through `EnemyData_20.asset`
- 20 wave configs: `Wave_01.asset` through `Wave_20.asset`

---

## Migration Notes

### If You Already Have Old Assets

**Option 1: Rename Existing Assets**
- Rename `Ground.prefab` → `Enemy_1.prefab`
- Rename `Flying.prefab` → `Enemy_5.prefab`
- Rename `GroundEnemy_Wave1.asset` → `EnemyData_1.asset`
- Rename `FlyingEnemy_Wave5.asset` → `EnemyData_5.asset`

**Option 2: Use Fallback System**
- Delete old assets
- System will auto-generate fallback data
- Create new assets incrementally as needed

### If WaveConfigData Assets Still Reference EnemyData

The old `EnemyData` field in WaveConfigData is no longer used. You can:
1. Leave it empty (system ignores it)
2. Delete and recreate WaveConfigData assets without the field

---

## Testing Checklist

### Verify Wave-Based Loading Works

1. **Create 3 test waves** (1, 2, 5)
   - [ ] Create `Enemy_1.prefab`, `Enemy_2.prefab`, `Enemy_5.prefab`
   - [ ] Create `EnemyData_1.asset`, `EnemyData_2.asset`, `EnemyData_5.asset`
   - [ ] Create `Wave_01.asset`, `Wave_02.asset`, `Wave_05.asset`

2. **Test Wave 1:**
   - [ ] Place 5 towers, select 1
   - [ ] Verify wave 1 spawns enemies with `Enemy_1.prefab` visual
   - [ ] Check console: "Loaded enemy model for wave 1"

3. **Test Wave 2:**
   - [ ] Complete wave 1, place 5 more towers, select 1
   - [ ] Verify wave 2 spawns enemies with `Enemy_2.prefab` visual
   - [ ] Check console: "Loaded enemy model for wave 2"

4. **Test Wave 5 (Flying):**
   - [ ] Complete waves 1-4
   - [ ] Verify wave 5 spawns enemies with `Enemy_5.prefab` visual
   - [ ] Verify flying enemies ignore towers in pathfinding

5. **Test Fallback System:**
   - [ ] Complete wave 5 without creating Wave_06.asset or EnemyData_6.asset
   - [ ] Check console: "Wave config not found, using fallback"
   - [ ] Check console: "Enemy data not found, using fallback"
   - [ ] Verify wave 6 still spawns with procedural stats

---

## Console Log Examples

### Successful Wave Loading:
```
Starting wave 1
Loaded enemy model for wave 1 from Prefabs/Enemies/Models/Enemy_1
Spawned enemy 1/5
...
```

### Fallback Model Loading:
```
Starting wave 6
Failed to load enemy model at Prefabs/Enemies/Models/Enemy_6
Created fallback enemy data for wave 6
Spawned enemy 1/7
```

### Missing Assets (Uses Fallback):
```
Wave config not found at ScriptableObjects/Waves/Wave_10, using fallback
Enemy data not found at ScriptableObjects/Enemies/EnemyData_10, using fallback
Created fallback enemy data for wave 10
```

---

## Future Enhancements

### Potential Additions:

1. **Model Pools by Enemy Type**
   - Load multiple models per wave (e.g., `Enemy_5_A.prefab`, `Enemy_5_B.prefab`)
   - Randomly select from pool for visual variety within a wave

2. **Wave Themes**
   - Group waves by theme (e.g., waves 1-5: slimes, 6-10: undead, 11-15: demons)
   - Share models within theme with material variations

3. **Boss Waves**
   - Special model naming: `Enemy_10_Boss.prefab`
   - Detect "Boss" suffix and spawn single high-health enemy

4. **Dynamic Enemy Scaling**
   - Read player performance metrics
   - Adjust fallback enemy stats based on win/loss rate

---

## Summary

The wave-based loading system provides:
- ✅ Clearer asset organization (numbered by wave)
- ✅ Unique visuals per wave without type restrictions
- ✅ Automatic fallback for missing assets
- ✅ Simplified asset creation workflow
- ✅ Better scalability for 20+ waves

**Key Takeaway:** Each wave number (1, 2, 3...) now has its own:
- `Enemy_{N}.prefab` (visual model)
- `EnemyData_{N}.asset` (stats)
- Loaded automatically by wave number in `WaveService`
