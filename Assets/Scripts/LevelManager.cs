using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level
{
    public int levelNumber;
    public int simpsToBeat, simpsBeaten;
    public int modsToBeat, modsBeaten;
    public int bossesToBeat, bossesBeaten;
}

[System.Serializable]
public class LevelScenes
{
    public int start = 0;
    public int end = 1;
    public List<int> sceneIndex = new List<int>();
}

public class LevelManager : Singleton<LevelManager>
{
    private TwitchClient twitchClient;
    public List<LevelScenes> levelScenes;
    public EnemyManager enemyManager;
    private int m_sceneLoadedIndex = -1;
    private CharacterControl m_player;
    public ModPowerBanner modBanner;
    public ViewerVoterManager viewerVoterManager;
    private BoxCollider m_spawnBox;

    // Data
    private Level currentLevelStats;
    private GameData data;
    private HashSet<SimpSpawnData> modsQueued = new HashSet<SimpSpawnData>();
    private HashSet<SimpSpawnData> simpsQueued = new HashSet<SimpSpawnData>();    
    private List<SimpSpawnData> modsSelected = new List<SimpSpawnData>();
    private List<SimpSpawnData> simpsSelected = new List<SimpSpawnData>();
    private List<SimpSpawnData> bossSelected = new List<SimpSpawnData>();

    [Header("Join")]
    [SerializeField]
    private GameObject m_joinMenu;
    [SerializeField]
    private GameObject m_joinTitle, m_stopTitle;
    [SerializeField]
    private UIElement m_joinCommandInfo;
    [SerializeField]
    private TextMeshProUGUI m_joinCommandText;
    private bool m_forceStart = false;

    [Header("Join - Timer")]
    [SerializeField]
    private TextMeshProUGUI m_joinTimeText;
    [SerializeField]
    private RectTransform m_timerContainer;
    private float m_joinTime = 120.0f;

    [Header("Join - Selecting")]
    [SerializeField]
    private GameObject m_joinedNamesMenu;
    [SerializeField]
    private RectTransform m_joinedNamesContainer;
    private List<NameJoined> m_namesThatJoined = new List<NameJoined>();

    private List<int> m_namesIndicesAvailable = new List<int>();
    [SerializeField]
    private GameObject m_forceJoinButton;
    [SerializeField]
    private GameObject m_twitchIcon;
    [SerializeField]
    FMODUnity.StudioEventEmitter m_countDownEvent;

    [Header("People stats")]
    [SerializeField]
    private TextMeshProUGUI m_totalPeopleJoined;
    [SerializeField]
    private TextMeshProUGUI m_totalSimpsJoined;
    [SerializeField]
    private TextMeshProUGUI m_totalModsJoined;

    [Header("Music")]
    public GameObject m_joinMusic;
    public GameObject m_combatMusic;
    public List<GameObject> m_bossMusic;

    [Header("Result Screens")]
    [SerializeField]
    private GameObject m_resultScreenComplete;
    [SerializeField]
    private GameObject m_resultScreenFailure;
    [SerializeField]
    private TextMeshProUGUI m_levelCompletedText;
    [SerializeField]
    private GameObject m_victoryScreen;
    [SerializeField, FMODUnity.EventRef]
    private string m_victoryCue;
    [SerializeField]
    private GameObject m_defeatScreen;
    [SerializeField, FMODUnity.EventRef]
    private string m_defeatCue;

    private bool m_onRetry = false;
    private bool m_onReturnHome = false;

    private bool m_isVictorious = false;
    private bool m_isDefeated = false;
    private bool m_hasContinued = false;

    [Header("Others")]
    public UpgradeManager upgradeManager;
    [SerializeField]
    private TextMeshProUGUI m_modsToBeat, m_simpsToBeat, m_currentLevel;

    private HashSet<string> m_peopleJoined;
    private HashSet<string> m_peopleVoted;

    public int currentLevel = 1;

    private void OnEnable()
    {
        m_player = FindObjectOfType<CharacterControl>();
        m_player.PreRunInit();
        m_peopleJoined = new HashSet<string>();
        m_peopleJoined.Clear();
        currentLevelStats = new Level();
        data = SaveData.current.gameData;
        twitchClient = FindObjectOfType<TwitchClient>();
        if (twitchClient == null)
            Debug.LogError("No twitch client found");
        viewerVoterManager.Initialize(twitchClient);
        m_joinCommandText.text = "Type !" + PlayerPrefs.GetString(EPlayerPrefParams.command.ToString(), "simp") + " to join";

        int counter = 0;
        foreach(Transform t in m_joinedNamesContainer)
        {
            NameJoined nj = t.GetComponent<NameJoined>();
            if (nj != null)
            {
                nj.Hide();
                m_namesThatJoined.Add(nj);
                m_namesIndicesAvailable.Add(counter);
                counter++;
            }
        }

        upgradeManager.Hide();
        StartCoroutine(CO_JoinCountdown());
    }

    private void CalculateCurrentLevelStats(int _level)
    {
        int baseSpawn = 2;
        float difficultySpike = 5.0f;
        currentLevelStats = new Level()
        {
            levelNumber = _level,
            simpsBeaten = 0, modsBeaten = 0, bossesBeaten = 0,
            simpsToBeat = baseSpawn + _level - Mathf.FloorToInt(_level / (difficultySpike * Mathf.CeilToInt(_level / difficultySpike))) + Mathf.FloorToInt((_level - 1) / 5) * 3,
            modsToBeat = Mathf.CeilToInt(_level / 5.0f),
            bossesToBeat = (_level % difficultySpike == 0) ? 1 : 0,
        };

        if (GAMESTATS.spawnMulti > 0)
        {
            currentLevelStats.simpsToBeat *= GAMESTATS.spawnMulti;
            currentLevelStats.modsToBeat *= GAMESTATS.spawnMulti;
        }
        m_modsToBeat.text = currentLevelStats.modsToBeat.ToString();
        m_simpsToBeat.text = currentLevelStats.simpsToBeat.ToString();
        m_currentLevel.text = currentLevelStats.levelNumber.ToString();
        m_isVictorious = false;
    }

    private void PrepareToJoin()
    {
        m_timerContainer.localScale = Vector3.zero;
        m_timerContainer.DOScale(Vector3.one, 0.2f).SetUpdate(true).SetEase(Ease.Linear);

        GameManager.Inst.controls.Player.Disable();
        twitchClient.canJoin = true;
        m_forceStart = false;
        m_joinMenu.SetActive(true);
        m_stopTitle.SetActive(false);
        m_joinTitle.SetActive(true);
        m_forceJoinButton.SetActive(true);
        m_joinCommandInfo.Show();
        m_joinedNamesMenu.SetActive(false);
        m_joinMusic.SetActive(true);
        m_combatMusic.SetActive(false);
        foreach (var music in m_bossMusic)
            music.SetActive(false);
        m_victoryScreen.SetActive(false);
        m_defeatScreen.SetActive(false);
        m_resultScreenComplete.SetActive(false);
        m_resultScreenFailure.SetActive(false);
        m_forceStart = false;
        m_hasContinued = false;
        m_isDefeated = false;
        m_isVictorious = false;
        m_onRetry = false;
        m_onReturnHome = false;
        m_peopleVoted = new HashSet<string>();
        Time.timeScale = 0.0f;
    }

    public void ForceStart()
    {
        m_forceStart = true;
    }

    IEnumerator CO_JoinCountdown()
    {
        GameManager.Inst.controls.Enable();
        CalculateCurrentLevelStats(currentLevel);
        PrepareToJoin();
        m_countDownEvent.SetParameter("End", 0);
        m_countDownEvent.Play();
        float timer = 0.0f;
        while(timer <= m_joinTime && !m_forceStart)
        {
            timer += Time.unscaledDeltaTime;
            m_joinTimeText.text = (m_joinTime - timer).ToString("F0");
            yield return null;
        }

        m_countDownEvent.SetParameter("End", 1);
        m_joinCommandInfo.Hide();
        m_joinTitle.SetActive(false);
        m_forceJoinButton.SetActive(false);
        m_stopTitle.SetActive(true);
        m_timerContainer.DOScale(Vector3.zero, 0.2f).SetUpdate(true).SetEase(Ease.Linear);
        twitchClient.canJoin = false;

        yield return new WaitForSecondsRealtime(3.0f);

        m_stopTitle.SetActive(false);
        m_joinMenu.SetActive(false);
        m_joinedNamesMenu.SetActive(true);
        simpsSelected.Clear();
        modsSelected.Clear();
        bossSelected.Clear();

        // Select viewers
        while (simpsSelected.Count < currentLevelStats.simpsToBeat && simpsQueued.Count != 0 && simpsQueued.Count != simpsSelected.Count)
        {
            SimpSpawnData simp = simpsQueued.Random();
            while (simpsSelected.Contains(simp))
                simp = simpsQueued.Random();

            AddSelectedSimp(simp);
            yield return new WaitForSecondsRealtime(0.1f);
        }

        // Fill with bots if not enough viewers
        while(simpsSelected.Count < currentLevelStats.simpsToBeat)
        {
            AddSelectedSimp(new SimpSpawnData() {
                userName = "Bot" + simpsSelected.Count
            });
            yield return new WaitForSecondsRealtime(0.1f);
        }

        // Select mods
        while(modsSelected.Count < currentLevelStats.modsToBeat && modsQueued.Count != 0 && modsQueued.Count != modsSelected.Count)
        {
            SimpSpawnData simp = modsQueued.Random();
            while (modsSelected.Contains(simp))
                simp = modsQueued.Random();

            AddSelectedSimp(simp);
            yield return new WaitForSecondsRealtime(0.1f);
        }

        // Fill with bots if not enough mods
        while (modsSelected.Count < currentLevelStats.modsToBeat)
        {
            AddSelectedSimp(new SimpSpawnData()
            {
                userName = "ModBot" + modsSelected.Count,
                isMod = true,
                // powers = new HashSet<ETwitchSettings>() { ETwitchSettings.MOD_FRIENDLY },
            });
            yield return new WaitForSecondsRealtime(0.1f);
        }

        if (currentLevelStats.bossesToBeat > 0)
        {
            foreach (var simp in modsQueued)
            {
                if (Extensions.ChanceRoll(70.0f))
                {
                    bossSelected.Add(simp);
                }
            }

            foreach (var simp in simpsSelected)
            {
                if (Extensions.ChanceRoll(20.0f))
                {
                    bossSelected.Add(simp);
                }
            }
        }

        yield return new WaitForSecondsRealtime(2.0f);

        bool doneLoading = false;
        LevelScenes scenesToLoad = GetCurrentLevelScenes(currentLevel);
        m_sceneLoadedIndex = scenesToLoad.sceneIndex.GetRandom();
        SceneLoader.Inst.LoadGameScene(m_sceneLoadedIndex, () => {
            doneLoading = true;
        });
        while (!doneLoading)
            yield return null;

        PostProcessingManager.Inst.Initialize();

        m_twitchIcon.SetActive(false);
        m_joinedNamesMenu.SetActive(false);
        m_joinMusic.SetActive(false);
        if (currentLevel % 5 == 0)
            m_bossMusic.GetRandom().SetActive(true);
        else
            m_combatMusic.SetActive(true);
        Time.timeScale = 1.0f;
        GameManager.Inst.controls.Player.Enable();

        var spawnArea = GameObject.FindGameObjectWithTag("SpawnArea");
        if (spawnArea != null)
            m_spawnBox = spawnArea.GetComponent<BoxCollider>();

        m_player.PreLevelInit();
        twitchClient.isPlaying = true;

        List<SimpSpawnData> simps = new List<SimpSpawnData>();
        simps.AddRange(modsSelected);
        simps.AddRange(simpsSelected);
        enemyManager.SpawnSimps(currentLevelStats, simps, bossSelected.GetRandom());
        viewerVoterManager.StartVoting();
    }

    public void JoinQueue(SimpSpawnData _simp)
    {
        if (m_peopleJoined.Contains(_simp.userName))
            return;
        m_peopleJoined.Add(_simp.userName);

        if (_simp.isMod)
            modsQueued.Add(_simp);
        else
            simpsQueued.Add(_simp);

        m_totalPeopleJoined.text = (modsQueued.Count + simpsQueued.Count).ToString();
        m_totalModsJoined.text = modsQueued.Count.ToString();
        m_totalSimpsJoined.text = simpsQueued.Count.ToString();
    }

    private void AddSelectedSimp(SimpSpawnData _simp)
    {
        if (_simp.isMod)
            modsSelected.Add(_simp);
        else
            simpsSelected.Add(_simp);

        int index = m_namesIndicesAvailable.GetRandom();
        m_namesIndicesAvailable.Remove(index);
        m_namesThatJoined[index].Show(_simp.userName, _simp.isMod);
        if(m_namesIndicesAvailable.Count == 0)
        {
            for (int i = 0; i < m_namesThatJoined.Count; i++)
            {
                m_namesIndicesAvailable.Add(i);
            }
        }
    }

    private void Update()
    {
        CheckVictory();
    }

    private void CheckVictory()
    {
        if(currentLevelStats.simpsBeaten >= currentLevelStats.simpsToBeat && !m_isVictorious && 
            currentLevelStats.modsBeaten >= currentLevelStats.modsToBeat &&
            currentLevelStats.bossesBeaten >= currentLevelStats.bossesToBeat)
        {
            m_isVictorious = true;
            StartCoroutine(CO_LevelComplete());
        }
    }

    private IEnumerator CO_LevelComplete()
    {
        GameManager.Inst.controls.Disable();
        viewerVoterManager.ForceStop();
        twitchClient.isPlaying = false;
        EventManager.onLevelComplete?.Invoke(currentLevel);

        m_victoryScreen.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot(m_victoryCue);
        m_combatMusic.SetActive(false);
        yield return new WaitForSecondsRealtime(3.5f);

        PlayerInfoWindow playerui = (PlayerInfoWindow)UIManager.Inst.uiWindows[EUiWindows.PlayerInformation];
        playerui.Hide();

        Time.timeScale = 0.0f;
        m_joinMusic.SetActive(true);
        m_victoryScreen.SetActive(false);
        m_twitchIcon.SetActive(true);

        simpsQueued.Clear();
        modsQueued.Clear();
        m_peopleJoined.Clear();
        m_totalPeopleJoined.text = (modsQueued.Count + simpsQueued.Count).ToString();
        m_totalModsJoined.text = modsQueued.Count.ToString();
        m_totalSimpsJoined.text = simpsQueued.Count.ToString();

        foreach (var name in m_namesThatJoined)
            name.Hide();

        m_resultScreenComplete.SetActive(true);
        m_levelCompletedText.text = "Level " + currentLevel.ToString() + " completed";

        SceneLoader.Inst.UnloadGameScene(m_sceneLoadedIndex);

        while (!m_hasContinued)
            yield return null;

        m_resultScreenComplete.SetActive(false);
        upgradeManager.Show(currentLevel);

        while(!upgradeManager.hasSelectedUpgrade)
                yield return null;

        upgradeManager.Hide();
        if (currentLevel > data.levelBeaten)
            data.levelBeaten = currentLevel;
        currentLevel++;

        SerializationManager.Save(SaveData.current);
        StartCoroutine(CO_JoinCountdown());
    }

    public void OnLevelContinue()
    {
        m_hasContinued = true;
    }

    public void OnDefeat()
    {
        m_isDefeated = true;
        enemyManager.SetAllSimpsTarget(null);
        StartCoroutine(CO_LevelFailure());
    }

    private IEnumerator CO_LevelFailure()
    {
        GameManager.Inst.controls.Disable();
        m_isDefeated = true;
        viewerVoterManager.ForceStop();
        enemyManager.HideBossBar();
        twitchClient.isPlaying = false;
        EventManager.onLevelFailed?.Invoke(currentLevel);

        m_defeatScreen.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot(m_defeatCue);
        m_combatMusic.SetActive(false);
        yield return new WaitForSecondsRealtime(4.9f);

        PlayerInfoWindow playerui = (PlayerInfoWindow)UIManager.Inst.uiWindows[EUiWindows.PlayerInformation];
        playerui.Hide();

        Time.timeScale = 0.0f;
        m_joinMusic.SetActive(true);
        m_defeatScreen.SetActive(false);
        m_twitchIcon.SetActive(true);

        foreach (var name in m_namesThatJoined)
            name.Hide();

        m_resultScreenFailure.SetActive(true);

        SceneLoader.Inst.UnloadGameScene(m_sceneLoadedIndex);

        while (!m_onReturnHome && !m_onRetry)
            yield return null;

        Time.timeScale = 1.0f;
        if (m_onReturnHome)
            SceneLoader.Inst.LoadScene(ESceneIndices.Home);

        if (m_onRetry)
        {
            StartCoroutine(CO_JoinCountdown());
        }

        m_resultScreenFailure.SetActive(false);
    }

    public void VoteForUpgrade(string _user, string _text)
    {
        if (m_peopleVoted.Contains(_user))
            return;
        m_peopleVoted.Add(_user);

        upgradeManager.ReceiveVote(_text);
    }

    public void CommandFromSimps(string _user, ETwitchSettings _power, string _option = "", bool _isHainki = false)
    {
        Simp simp = enemyManager.GetSimpByName(_user);
        if(simp)
        {
            switch (_power)
            {
                case ETwitchSettings.MOD_MINIVERSION:
                    if (!simp.isFriendly)
                    {
                        simp.TryAndUseSkill("minime", 2.0f, () =>
                        {
                            var minis = enemyManager.SpawnSmallSimps(simp, 1, "Mini ");
                            
                            foreach (var m in minis)
                            {
                                m.MakeGnome();
                                currentLevelStats.modsToBeat++;
                            }
                        });
                    }
                    break;
                case ETwitchSettings.MOD_DESPAWN:
                    if(!simp.isBoss)
                        simp.TakeDamage(9999.0f, Vector3.up, true);
                    break;
                case ETwitchSettings.MOD_SHIELD:
                    simp.TryAndUseSkill("shield", 1.5f, () =>
                    {
                        simp.shieldObject.SetActive(true);
                    });
                    break;
                case ETwitchSettings.MOD_SHOOT:
                    simp.TryAndUseSkill("shoot", 1.5f, () =>
                    {
                        simp.Shoot();
                    });
                    break;
                case ETwitchSettings.MOD_SPECIAL:
                    ModSimp ms = (ModSimp)simp;
                    if(ms == null)
                    {
                        return;
                    }
                    ms.TryAndUseSkill("special", 15.0f, () =>
                    {
                        if (GetModName(_user, out EModNames _mod))
                        {
                            modBanner.PlayAnimation(_mod, () => 
                            {
                                ms.UseSpecialPower(_option);
                            });
                        }
                    }, _isHainki);
                    break;
                default:
                    break;
            }
        }
    }

    private bool GetModName(string _username, out EModNames _modName)
    {
        if (_username == "Kofupoka")
        {
            _modName = EModNames.Kofu;
            return true;
        }
        else if (_username == "Nykz420")
        {
            _modName = EModNames.Nykz;
            return true;
        }
        else if (_username == "Separatedice")
        {
            _modName = EModNames.Separatedice;
            return true;
        }
        else if (_username == "Slyde_")
        {
            _modName = EModNames.Slyde;
            return true;
        }
        else if (_username == "Undeadbanana")
        {
            _modName = EModNames.Undeadbanana;
            return true;
        }

        _modName = EModNames.Kofu;
        return false;
    }

    public void OnReturnHome()
    {
        m_onReturnHome = true;
    }

    public void OnRetry()
    {
        m_onRetry = true;
    }

    private LevelScenes GetCurrentLevelScenes(int _levelIndex)
    {
        LevelScenes result = levelScenes.Find((LevelScenes _scenes) =>
        {
            return (_scenes.start <= _levelIndex && _levelIndex <= _scenes.end);
        });

        return result;
    }

    public Transform GetAliveSimp(Transform _simpAsking)
    {
        return enemyManager.GetAllAlliveSimps(_simpAsking).GetRandom();
    }

    public List<Simp> GetSimpsInRadius(Vector3 _position, float _radius)
    {
        return enemyManager.GetSimpsInRadius(_position, _radius);
    }

    public void VoteForInGameEffect(string _vote)
    {
        viewerVoterManager.CountVote(_vote);
    }

    public bool CanStartNewVote()
    {
        return !m_isVictorious && !m_isDefeated;
    }

    public void SpawnRandomSimp()
    {
        SimpSpawnData simp = simpsQueued.Random();
        if (simp == null)
            simp = new SimpSpawnData() { isMod = false, userName = "Reeeeeeeee", powers = new HashSet<ETwitchSettings>() };
        else
            simpsQueued.Remove(simp);
        enemyManager.SpawnRandomSimp(simp);
        currentLevelStats.simpsToBeat++;
    }

    public void SpawnCustomSimp(SimpSpawnData _data)
    {
        enemyManager.SpawnRandomSimp(_data);
        if (_data.isMod)
            currentLevelStats.modsToBeat++;
        else
            currentLevelStats.simpsToBeat++;
    }

    [Button]
    public void SimpRain(int _amount = 10, float _time = 0.2f)
    {
        if (m_spawnBox == null) return;
        StartCoroutine(CO_SimpRain(_amount, _time));
    }

    private IEnumerator CO_SimpRain(int _amount, float _timePerSimp = 0.2f)
    {
        for (int i = 0; i < _amount; i++)
        {
            var obj = PoolManager.Inst.GetObject(EPoolObjectType.PEPE_RAGDOLL);
            PepeRagdollThrow pepe = obj.GetComponent<PepeRagdollThrow>();
            pepe.Throw(m_spawnBox.GetRandomPositionInCollider(), Vector3.down, 50.0f);
            yield return new WaitForSeconds(_timePerSimp);
        }
    }

    public void ScreenEffect(int _effect)
    {
        viewerVoterManager.PostProcessingEffect(_effect);
    }
}
