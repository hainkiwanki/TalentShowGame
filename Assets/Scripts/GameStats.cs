using System.Collections.Generic;
using UnityEngine;

public static class GAMESTATS
{
    private static GameStats currentStats;

    // Game
    public static bool canSpawnGnomes => currentStats.gameStats[ERunStat.GNOMES].value > 0;
    public static int amountOfGnomes => Mathf.RoundToInt(currentStats.gameStats[ERunStat.GNOMES].value);
    public static int spawnMulti => (int)currentStats.gameStats[ERunStat.SPAWN_AMOUNT_MODIFIER].value;
    public static bool isJebaited => currentStats.gameStats[ERunStat.START_ON_1HP].value > 0;

    // Simps + mods
    public static float simpMoveSpeedMulti => 1.0f + currentStats.gameStats[ERunStat.SIMP_SPEED].value;
    public static float modMoveSpeedMulti => 1.0f + currentStats.gameStats[ERunStat.MOD_SPEED].value;
    public static float simpDamageMulti => 1.0f + currentStats.gameStats[ERunStat.SIMP_DAMAGE].value;
    public static float modDamageMulti => 1.0f + currentStats.gameStats[ERunStat.MOD_DAMAGE].value;
    public static float simpHealthMulti => 1.0f + currentStats.gameStats[ERunStat.SIMP_HEALTH].value;
    public static float modHealthMulti => 1.0f + currentStats.gameStats[ERunStat.MOD_HEALTH].value;

    // Player
    public static float maxHealth => currentStats.playerStats[ECharacterStat.MAXHEALTH].value;
    public static float chargeTime => currentStats.playerStats[ECharacterStat.CHARGE_TIME].value;
    public static float damage => currentStats.playerStats[ECharacterStat.DAMAGE_DEALT].value;
    public static float damageTakenMulti => currentStats.playerStats[ECharacterStat.DAMAGE_TAKEN_MULTIPLIER].value;
    public static float moneyMulti => 1 + moneyPercentage;
    public static float moneyPercentage => currentStats.playerStats[ECharacterStat.GOLD_MULTIPLIER].value;
    public static float attackSpeed => currentStats.playerStats[ECharacterStat.ATTACKSPEED].value;
    public static float moveSpeed => currentStats.playerStats[ECharacterStat.MOVEMENTSPEED].value;
    public static float moveMulti => moveSpeed / currentStats.playerStats[ECharacterStat.MOVEMENTSPEED].baseValue;
    public static float bulletSize => currentStats.playerStats[ECharacterStat.BULLET_SIZE].value;
    public static float lifeSteal => currentStats.playerStats[ECharacterStat.LIFESTEAL].value;
    public static float throwForce => currentStats.playerStats[ECharacterStat.THROW_FORCE].value;
    public static bool canAyaya => currentStats.playerStats[ECharacterStat.AYAYA].value > 0;
    public static bool canCurse => currentStats.playerStats[ECharacterStat.BRITISH_SLANG].value > 0;
    
    // Twitch settings
    public static bool shouldTimeout => currentStats.twitchSettings.Contains(ETwitchSettings.TIMEOUT);
    public static float timeoutTime => currentStats.twitchTimeoutTime;
    public static bool modFriendlyEnabled => currentStats.twitchSettings.Contains(ETwitchSettings.MOD_FRIENDLY);
    public static bool modCanSpawnMinis => currentStats.twitchSettings.Contains(ETwitchSettings.MOD_MINIVERSION);
    public static bool modCanDespawn => currentStats.twitchSettings.Contains(ETwitchSettings.MOD_DESPAWN);
    public static bool modCanShield => currentStats.twitchSettings.Contains(ETwitchSettings.MOD_SHIELD);
    public static bool modCanShoot => currentStats.twitchSettings.Contains(ETwitchSettings.MOD_SHOOT);
    public static bool modCanBeBoss => currentStats.twitchSettings.Contains(ETwitchSettings.MOD_BOSS);

    public static bool simpCanBeAttacker => currentStats.twitchSettings.Contains(ETwitchSettings.SIMP_ATTACK);
    public static bool simpCanBeDefender => currentStats.twitchSettings.Contains(ETwitchSettings.SIMP_TANK);
    public static bool simpCanBeChad => currentStats.twitchSettings.Contains(ETwitchSettings.SIMP_HUGE);
    public static bool simpCanBeTransparent => currentStats.twitchSettings.Contains(ETwitchSettings.SIMP_TRANSPARENT);


    public static void SetGamestats(GameStats _stats)
    {
        currentStats = _stats;
    }

    public static void AddPlayerUpgrade(PlayerUpgrade _upgrade)
    {
        foreach (var _mod in _upgrade.modifiers)
        {
            currentStats.playerStats[_mod.stat].AddModifier(_mod.modifer);
            EventManager.onPlayerStatModiferAdded?.Invoke(_mod);
        }
    }

    public static void RemovePlayerUpgrade(PlayerUpgrade _upgrade)
    {

    }

    public static void AddRunUpgrade(RunUpgrade _upgrade)
    {
        currentStats.AddRunUpgrade(_upgrade.modifiers, _upgrade.goldModifer);
    }

    public static void RemoveRunUpgrade(RunUpgrade _upgrade)
    {
        currentStats.RemoveUpgrade(_upgrade.modifiers, _upgrade.goldModifer);
    }
    
    public static void ResetPlayerStats()
    {
        currentStats.playerStats = new Dictionary<ECharacterStat, Stat>()
        {
            { ECharacterStat.MOVEMENTSPEED, new Stat(10.0f, 5.0f, 20.0f) },
            { ECharacterStat.DAMAGE_DEALT, new Stat(4.0f) },
            { ECharacterStat.DAMAGE_TAKEN_MULTIPLIER, new Stat(1.0f) },
            { ECharacterStat.MAXHEALTH, new Stat(10.0f) },
            { ECharacterStat.LIFESTEAL, new Stat(0.0f) },
            { ECharacterStat.GOLD_MULTIPLIER, new Stat(0.0f) },
            { ECharacterStat.BULLET_SIZE, new Stat(1.0f, 0.1f, 5.0f) },
            { ECharacterStat.CHARGE_TIME, new Stat(3.0f, 1.0f, 5.0f) },
            { ECharacterStat.ATTACKSPEED, new Stat(1.0f, 0.1f, 2.0f) },
            { ECharacterStat.THROW_FORCE, new Stat(15.0f, 1.0f, 500.0f) },
            { ECharacterStat.BRITISH_SLANG, new Stat(0.0f) },
            { ECharacterStat.AYAYA, new Stat(0.0f) },
        };
    }
}

public class GameStats : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<ECharacterStat, Stat> playerStats;
    [HideInInspector]
    public Dictionary<ERunStat, Stat> gameStats;
    [HideInInspector]
    public HashSet<ETwitchSettings> twitchSettings;
    [HideInInspector]
    public float twitchTimeoutTime;

    [HideInInspector]
    public GameData gameData;

    private void Awake()
    {
        GAMESTATS.SetGamestats(this);
        EventManager.onSimpSpawned += OnSimpSpawned;
        EventManager.onSimpBeaten += OnSimpBeaten;
        EventManager.onBossBeaten += OnBossBeaten;
        EventManager.onLevelComplete += OnLevelComplete;
        EventManager.onLevelFailed += OnLevelFailed;
    }

    public void Initialize(SaveData _saveData)
    {
        gameData = _saveData.gameData;
        gameStats = new Dictionary<ERunStat, Stat>()
        {
            { ERunStat.GNOMES, new Stat(0) },
            { ERunStat.NO_HEALTHGAIN, new Stat(0) },
            { ERunStat.SPAWN_AMOUNT_MODIFIER, new Stat(1) },
            { ERunStat.START_ON_1HP, new Stat(0) },
            { ERunStat.MOD_DAMAGE, new Stat(1) },
            { ERunStat.MOD_HEALTH, new Stat(1) },
            { ERunStat.MOD_SPEED, new Stat(0) },
            { ERunStat.SIMP_DAMAGE, new Stat(0) },
            { ERunStat.SIMP_HEALTH, new Stat(0) },
            { ERunStat.SIMP_SPEED, new Stat(0) },
        };

        twitchSettings = _saveData.gameData.twitchRunSettings;
        twitchTimeoutTime = _saveData.gameData.twitchTimeoutTime;
    }

    public void AddRunUpgrade(List<RunModifier> _mods, StatModifier _goldMod)
    {
        foreach(var mod in _mods)
        {
            gameStats[mod.stat].AddModifier(mod.modifer);
        }
        playerStats[ECharacterStat.GOLD_MULTIPLIER].AddModifier(_goldMod);
    }

    public void RemoveUpgrade(List<RunModifier> _mods, StatModifier _goldMod)
    {
        foreach(var mod in _mods)
        {
            gameStats[mod.stat].RemoveModifier(mod.modifer);
        }
        playerStats[ECharacterStat.GOLD_MULTIPLIER].RemoveModifier(_goldMod);
    }

    private void OnSimpSpawned(Simp _s)
    {
        if (!gameData.simpStatData.ContainsKey(_s.username))
        {
            gameData.simpStatData.Add(_s.username, new SimpData());
            gameData.simpStatData[_s.username].isMod = _s.isMod;
        }

        if (_s.isBoss)
            gameData.simpStatData[_s.username].spawnedBoss++;
        else
            gameData.simpStatData[_s.username].spawnedNormal++;
    }

    private void OnBossBeaten(Simp _s)
    {
        if(!gameData.simpStatData.ContainsKey(_s.username))
        {
            return;
        }

        gameData.simpStatData[_s.username].beatenBoss++;
    }

    private void OnSimpBeaten(Simp _s)
    {
        if (!gameData.simpStatData.ContainsKey(_s.username))
        {
            return;
        }

        gameData.simpStatData[_s.username].beatenNormal++;
        gameData.totalMoneyMade += _s.moneyDrop;
    }

    private void OnLevelComplete(int _i)
    {
        if (!gameData.levelCompleteAmount.ContainsKey(_i))
            gameData.levelCompleteAmount.Add(_i, 0);
        gameData.levelCompleteAmount[_i]++;
    }

    private void OnLevelFailed(int _i)
    {
        if (!gameData.levelFailedAmount.ContainsKey(_i))
            gameData.levelFailedAmount.Add(_i, 0);
        gameData.levelFailedAmount[_i]++;
    }
}