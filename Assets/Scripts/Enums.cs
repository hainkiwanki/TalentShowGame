public enum ENodeType
{
    Begin,
    Speak,
    Light,
    Question,
    End,
}
public enum ESpeakNodeType
{
    Audio,
    Text,
    AudioAndText
}

public enum EInteractableActions
{
    None,
    LoadScene,
    ShowUI,
}

public enum EUiWindows
{
    RunSettings, PlayerInformation, Pause, 
}

public enum ERunStat
{
    GNOMES,
    SPAWN_AMOUNT_MODIFIER,
    START_ON_1HP,
    NO_HEALTHGAIN,
    MOD_DAMAGE,
    SIMP_DAMAGE,
    MOD_HEALTH,
    SIMP_HEALTH,
    MOD_SPEED,
    SIMP_SPEED,
}

public enum ECharacterStat
{
    MOVEMENTSPEED,
    DAMAGE_DEALT,
    DAMAGE_TAKEN_MULTIPLIER,
    MAXHEALTH,
    LIFESTEAL,
    GOLD_MULTIPLIER,
    BULLET_SIZE,
    CHARGE_TIME,
    ATTACKSPEED,
    THROW_FORCE,
    BRITISH_SLANG,
    AYAYA,
}

public enum ETwitchSettings
{
    TIMEOUT,

    MOD_FRIENDLY,
    MOD_MINIVERSION,    // SPAWN MINI VERSION
    MOD_DESPAWN,
    MOD_SHIELD,
    MOD_SHOOT,
    MOD_BOSS,
    MOD_SPECIAL,

    // CLASSES
    SIMP_HUGE,
    SIMP_TRANSPARENT,
    SIMP_TANK,
    SIMP_ATTACK,
}

public enum EStatModifierType
{
    FLAT = 0, PERCENT, MULTIPLIER, NEG_PERCENT,
}