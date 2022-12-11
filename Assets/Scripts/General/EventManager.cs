using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public delegate void OnSceneLoaded(int _index);
    public static OnSceneLoaded onSceneLoaded;

    public delegate void OnTwitchConnection(bool _connected);
    public static OnTwitchConnection onTwitchConnection;

    public delegate void OnSimpBeaten(Simp _s);
    public static OnSimpBeaten onSimpBeaten;

    public delegate void OnBossBeaten(Simp _s);
    public static OnBossBeaten onBossBeaten;

    public delegate void OnSimpSpawned(Simp _s);
    public static OnSimpSpawned onSimpSpawned;

    public delegate void OnDamageDealt();
    public static OnDamageDealt onDamageDealt;

    public delegate void OnLevelComplete(int _level);
    public static OnLevelComplete onLevelComplete;

    public delegate void OnLevelFailed(int _level);
    public static OnLevelFailed onLevelFailed;


    public delegate void OnPlayerStatModiferAdded(PlayerModifier _mod);
    public static OnPlayerStatModiferAdded onPlayerStatModiferAdded;
}

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }