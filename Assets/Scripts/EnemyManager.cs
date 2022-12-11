using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SerializedMonoBehaviour
{
    private Level levelData;
    private List<Transform> simpSpawnLocations;
    private Transform bossSpawnLocation;

    public Material customColorMat;
    public Material transparentMat;
    public Material spawnMat;

    private List<Simp> allSimps;

    public RectTransform simpsStatsParent;

    public BossBar bossBar;
    public VersusBanner versusBanner;

    public Dictionary<string, ModSimp> modSpecificSimps = new Dictionary<string, ModSimp>();

    [SerializeField, FMODUnity.EventRef]
    private string m_gnomeSfx;

    private void Awake()
    {
        EventManager.onSimpBeaten += OnSimpBeaten;
        EventManager.onBossBeaten += OnBossBeaten;
    }

    private void OnDestroy()
    {
        EventManager.onSimpBeaten -= OnSimpBeaten;
        EventManager.onBossBeaten -= OnBossBeaten;
    }

    public void SpawnSimps(Level _currentLevel, List<SimpSpawnData> _simpsToSpawn, SimpSpawnData _boss)
    {
        ResetDisplays();
        allSimps = new List<Simp>();
        simpSpawnLocations = new List<Transform>();
        bossSpawnLocation = null;
        levelData = _currentLevel;

        GameObject parent = GameObject.FindGameObjectWithTag("SpawnLocationParent");
        if(parent == null)
        {
            Debug.Log("No spawn locations in the scene!");
            return;
        }

        Transform spawnLocations = parent.GetComponent<Transform>();
        bossSpawnLocation = spawnLocations.GetChild(0);
        foreach(Transform t in spawnLocations)
        {
            if (t != spawnLocations && t != bossSpawnLocation)
                simpSpawnLocations.Add(t);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
        }

        if(GAMESTATS.canSpawnGnomes)
        {
            levelData.modsToBeat += levelData.modsToBeat * GAMESTATS.amountOfGnomes;
            levelData.simpsToBeat += levelData.simpsToBeat * GAMESTATS.amountOfGnomes;
        }

        StartCoroutine(CO_SpawnSimpWaves(_simpsToSpawn, _boss));
    }

    IEnumerator CO_SpawnSimpWaves(List<SimpSpawnData> _simps, SimpSpawnData _boss)
    {
        SimpSpawnData bossData = _boss;
        if(bossData == null)
        {
            var wannebeBosses = _simps.FindAll((SimpSpawnData simpData) =>
            {
                return simpData.powers.Contains(ETwitchSettings.MOD_BOSS);
            });
            bossData = wannebeBosses.GetRandom();
            if(bossData != null)
            {
                _simps.Remove(bossData);
                levelData.modsToBeat--;
                levelData.bossesToBeat++;
            }
        }
        if(bossData != null)
        {
            var simp = SpawnSimp(bossData, bossSpawnLocation.position);
            simp.InitializeAsBoss(GameManager.Inst.playerControl.transform, bossData.userName, bossBar, LevelManager.Inst.currentLevel);
            allSimps.Add(simp);
            EventManager.onSimpSpawned?.Invoke(simp);
            versusBanner.Show(bossData.userName);
        }
        while(_simps.Count > 0)
        {
            int minSpawns = 4;
            int maxSpawns = 6;
            int min = Mathf.Min(minSpawns, _simps.Count);
            int max = Mathf.Min(maxSpawns, _simps.Count);
            int spawns = Random.Range(min, max);

            for(int i = 0; i < spawns; i++)
            {
                var data = _simps.RemoveRandom();
                var simp = SpawnSimp(data, simpSpawnLocations.GetRandom().position);
                allSimps.Add(simp);
                EventManager.onSimpSpawned?.Invoke(simp);
                yield return new WaitForSeconds(1.0f);
            }

            float timer = spawns * 1.5f;
            while(timer > 0 && AreAnySimpsAlive())
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        yield return null;
    }

    private bool AreAnySimpsAlive()
    {
        foreach(var s in allSimps)
        {
            if (!s.isDead)
                return true;
        }

        return false;
    }

    private bool HasModSimpModel(string _username)
    {
        return modSpecificSimps.ContainsKey(_username);
    }

    [Button]
    private void SpawnCustomSimp(string _name)
    {
        var simp = SpawnSimp(new SimpSpawnData() { userName = _name, isMod = true, powers = new HashSet<ETwitchSettings>() }, simpSpawnLocations.GetRandom().position);
        allSimps.Add(simp);
        levelData.modsToBeat++;
    }

    private Simp SpawnSimp(SimpSpawnData _data, Vector3 _pos)
    {
        Simp simp;
        if (!HasModSimpModel(_data.userName))
        {
            var simpGO = PoolManager.Inst.GetObject(EPoolObjectType.SIMP);
            simpGO.SetActive(true);
            simp = SpawnSimpOfClass<NormalSimp>(simpGO, _data);
        }
        else
        {
            var simpGO = Instantiate(modSpecificSimps[_data.userName]);
            simpGO.gameObject.SetActive(true);
            simp = SpawnSimpOfClass<ModSimp>(simpGO.gameObject, _data);
        }
        InitializeSimp(simp, _pos, _data);
        return simp;

    }
    
    public void SpawnRandomSimp(SimpSpawnData _data)
    {
        var simp = SpawnSimp(_data, simpSpawnLocations.GetRandom().position);
        allSimps.Add(simp);
        EventManager.onSimpSpawned?.Invoke(simp);
    }

    private Simp SpawnSimpOfClass<T>(GameObject _simpObj, SimpSpawnData _data) where T : Simp
    {
        Simp simp = _simpObj.GetComponent<T>();
        simp.transform.localScale = Vector3.one;
        var statsGO = PoolManager.Inst.GetObject(EPoolObjectType.SIMPSTATS);
        statsGO.SetActive(true);
        statsGO.transform.parent = simpsStatsParent;
        SimpDisplayInfo stats = statsGO.GetComponent<SimpDisplayInfo>();
        simp.Initialize(GameManager.Inst.playerControl.transform, _data.userName, stats);

        if(simp is NormalSimp)
        {
            var normalSimp = (NormalSimp)simp;
            Material altMat = new Material(customColorMat);
            if (_data.powers.Contains(ETwitchSettings.SIMP_TRANSPARENT))
            {
                Destroy(altMat);
                altMat = new Material(transparentMat);
            }
            normalSimp.SetMaterials(new Material(spawnMat), altMat, !_data.powers.Contains(ETwitchSettings.SIMP_TRANSPARENT));
        }
        else if(simp is ModSimp)
        {
        }

        return simp;
    }

    private void InitializeSimp(Simp _simp, Vector3 _pos, SimpSpawnData _data)
    {
        _simp.SetHp(_data.isMod, levelData.levelNumber);
        _simp.SetMoney(_data.isMod, levelData.levelNumber);
        _simp.SetParticles(_data.isMod, _data.powers.Contains(ETwitchSettings.SIMP_ATTACK), _data.powers.Contains(ETwitchSettings.SIMP_TANK));
        if (_data.powers.Contains(ETwitchSettings.SIMP_HUGE))
        {
            _simp.MakeHuge(Vector3.one * 2.0f);
        }
        if (_data.powers.Contains(ETwitchSettings.SIMP_TRANSPARENT))
        {
            _simp.isTransparent = true;
        }
        _simp.canSpawnGnomes = GAMESTATS.canSpawnGnomes;

        if (_data.powers.Contains(ETwitchSettings.MOD_FRIENDLY) && _data.isMod)
        {
            _simp.MakeFriendly();
            levelData.modsToBeat--;
        }

        _simp.transform.position = _pos;
        _simp.transform.rotation = Quaternion.LookRotation(-_simp.transform.position.normalized);
    }

    private void OnSimpBeaten(Simp _s)
    {
        if (GAMESTATS.canSpawnGnomes && _s.canSpawnGnomes)
        {
            var gnomes = SpawnSmallSimps(_s, GAMESTATS.amountOfGnomes, "Gnome ");
            foreach(var gnome in gnomes)
            {
                gnome.canSpawnGnomes = false;
                gnome.MakeGnome();
                FMODUnity.RuntimeManager.PlayOneShot(m_gnomeSfx);
            }
        }    
        if (_s.isMod)
            levelData.modsBeaten++;
        else
            levelData.simpsBeaten++;
    }

    public List<Simp> SpawnSmallSimps(Simp _originaSimps, int _amt, string _namePrefix)
    {
        List<Simp> simpsSpawned = new List<Simp>();
        for (int i = 0; i < _amt; i++)
        {
            Vector3 startPos = _originaSimps.transform.position;
            float angle = 36.0f * i;
            float radius = 1.1f;
            startPos.x = startPos.x + radius * Mathf.Cos(angle);
            startPos.y = startPos.y + radius * Mathf.Sin(angle);

            SimpSpawnData simpData = new SimpSpawnData()
            {
                isMod = _originaSimps.isMod,
                powers = new HashSet<ETwitchSettings>(),
                userName = _namePrefix + _originaSimps.username,
            };
            var minisimp = SpawnSimp(simpData, startPos);
            simpsSpawned.Add(minisimp);
        }
        return simpsSpawned;
    }

    private void OnBossBeaten(Simp _s)
    {
        levelData.bossesBeaten++;
    }

    public void SetAllSimpsTarget(Transform _target)
    {
        foreach(Simp s in allSimps)
        {
            s.UpdateTarget(_target);
        }
    }

    public List<Transform> GetAllAlliveSimps(Transform _exclude)
    {
        List<Transform> aliveSimps = new List<Transform>();

        foreach (var simp in allSimps)
            if (!simp.isDead && simp.transform != _exclude)
                aliveSimps.Add(simp.transform);

        return aliveSimps;
    }

    public List<Simp> GetSimpsInRadius(Vector3 _position, float _radius)
    {
        List<Simp> results = new List<Simp>();

        foreach(var simp in allSimps)
        {
            if(!simp.isDead)
            {
                float distance = Vector3.Distance(simp.transform.position, _position);
                if(distance <= _radius)
                {
                    results.Add(simp);
                }
            }
        }

        return results;
    }

    public void ResetDisplays()
    {
        foreach (Transform info in simpsStatsParent)
        {
            var display = info.GetComponent<SimpDisplayInfo>();
            if (display != null)
                display.Hide();
        }
    }

    public Simp GetSimpByName(string _username)
    {
        foreach(var simp in allSimps)
        {
            if(!simp.isDead && simp.username == _username)
            {
                return simp;
            }
        }
        return null;
    }

    public void HideBossBar()
    {
        bossBar.Hide();
    }

    //public void PushBackSimps(Vector3 _position)
    //{
    //    float maxRadius = 12.0f;
    //    List<Simp> toPushBack = new List<Simp>();
    //    List<Simp> toMoveBack = new List<Simp>();
    //    for(int i = 0; i < spawnedSimps.Count; i++)
    //    {
    //        if (spawnedSimps[i] != null)
    //        {
    //            float distance = Vector3.Distance(_position, spawnedSimps[i].transform.position);
    //            if (distance < maxRadius)
    //            {
    //                if (spawnedSimps[i].isDead)
    //                {
    //                    toPushBack.Add(spawnedSimps[i]);
    //                }
    //                else
    //                {
    //                    toMoveBack.Add(spawnedSimps[i]);
    //                }
    //            }
    //        }
    //    }

    //    for(int i = 0; i < toMoveBack.Count; i++)
    //    {
    //        Vector3 fleeDir = (toMoveBack[i].transform.position - _position).normalized;
    //        float distance = Vector3.Distance(toMoveBack[i].transform.position, _position);
    //        if(distance < 1.0f)
    //        {
    //            fleeDir = new Vector3(
    //                    Random.Range(0, 1.0f),
    //                    0.0f,
    //                    Random.Range(0, 1.0f)
    //                ).normalized;
    //        }
    //        Vector3 fleePos = fleeDir * 10.0f;
    //        toMoveBack[i].SetFleePosition(fleePos);
    //    }

    //    for(int i = 0; i < toPushBack.Count; i++)
    //    {
    //        Vector3 pushDir = (toPushBack[i].transform.position - _position).normalized;
    //        float distance = Vector3.Distance(toPushBack[i].transform.position, _position);
    //        float force = Mathf.Lerp(100.0f, 500.0f, distance / maxRadius);
    //        toPushBack[i].PushBody(pushDir * force);
    //    }
    //}
}