using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SimpSpawnData
{
    public string userName = "";
    public bool isMod = false;
    public HashSet<ETwitchSettings> powers = new HashSet<ETwitchSettings>();
}

[ExecuteInEditMode]
public abstract class Simp : PoolObject
{
    public GameObject spawnParticleSystem;
    public List<Transform> bossAffectedTransforms;

    protected SimpDisplayInfo displayInfo;
    protected HashSet<Collider> m_ragdollParts;
    public Rigidbody body;
    public GameObject modParticle, attackerParticle, defenderParticle;

    protected float m_hp = 5.0f;
    protected float m_maxHp = 5.0f;
    [HideInInspector] public bool isMod = false;
    [HideInInspector] public bool hasSpawned = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public ulong moneyDrop = 0;
    [HideInInspector] public bool isGnome = false;
    [HideInInspector] public bool isBoss = false;
    [HideInInspector] public bool isFriendly = false;
    [HideInInspector] public bool isHuge = false;
    [HideInInspector] public bool isTransparent = false;
    [HideInInspector] public bool canSpawnGnomes = false;


    [SerializeField]
    protected Animator m_animator;
    public Animator animator => m_animator;

    public CharacterController charController
    {
        get
        {
            if (m_characterCollider == null)
                m_characterCollider = GetComponent<CharacterController>();
            return m_characterCollider;
        }
    }
    protected CharacterController m_characterCollider;

    public NavMeshAgent navAgent
    {
        get
        {
            if (m_navAgent == null)
                m_navAgent = GetComponent<NavMeshAgent>();
            return m_navAgent;
        }
    }
    protected NavMeshAgent m_navAgent;
    public bool hasTarget => m_target != null;
    public Transform target => m_target;
    protected Transform m_target;

    public List<SimpAttack> attackParticles = new List<SimpAttack>();

    public string username
    {
        get
        {
            if (!isBoss)
                return displayInfo.nameText.text;
            else
                return bossHealthBar.name;
        }
    }

    protected BossBar bossHealthBar;

    [SerializeField] protected ParticleSystem m_banHammerPs;
    [SerializeField] protected Transform m_banHammerObj;
    [SerializeField, FMODUnity.EventRef] protected string m_hammerSfx;
    protected float m_hammerCdr = 5.0f;
    [HideInInspector] public bool canHammerAttack = true;

    protected Dictionary<string, bool> m_canUseSkill;
    public GameObject shieldObject;
    public Transform simpBulletTransform;

    public Vector3 dirToTarget
    {
        get
        {
            if (!hasTarget)
                return Vector3.one;
            else
                return (m_target.position - transform.position).normalized;
        }
    }

    public void InitializeAsBoss(Transform _player, string _name, BossBar _bossBar, int _level)
    {
        InitializeReset();
        SetBossAffectTransforms(2.0f);
        isBoss = true;
        m_maxHp = _level * 100.0f;
        m_hp = m_maxHp;
        m_target = _player;
        bossHealthBar = _bossBar;
        bossHealthBar.ShowBar(_name);
        bossHealthBar.SetHealth(m_hp, m_maxHp, 2.0f);
        displayInfo?.Hide();
        moneyDrop *= 3;
        OnBossInitialize();
        GameManager.Inst.ExecuteFunctionWithDelay(5.0f, () =>
        {
            StartCoroutine(CO_ThrowSimp());
        });
    }

    public void Initialize(Transform _player, string _name, SimpDisplayInfo _stats)
    {
        InitializeReset();
        m_target = _player;
        displayInfo = _stats;
        displayInfo.target = this;
        displayInfo.SetHealth(1.0f);
        displayInfo.nameText.text = _name;
        displayInfo.yOffset = 40.0f;
        OnSimpInitialize();
    }

    private void InitializeReset()
    {
        SetRagdollParts();
        SetBossAffectTransforms(1.0f);
        shieldObject.SetActive(false);
        m_canUseSkill = new Dictionary<string, bool>();
        m_hammerCdr = 5.0f;
        canHammerAttack = true;
        m_banHammerObj.localScale = Vector3.zero;
        isDead = false;
        if(charController)
            charController.enabled = true;
        m_animator.enabled = true;
        navAgent.enabled = true;
        isGnome = false;
        isBoss = false;
        isFriendly = false;
        isHuge = false;
        isTransparent = false;
        canSpawnGnomes = false;
        hasSpawned = false;
        spawnParticleSystem.SetActive(true);
    }

    protected virtual void OnSimpInitialize() { }
    protected virtual void OnBossInitialize() { }

    public void SetParticles(bool _mod, bool _attack, bool _defense)
    {
        isMod = _mod;
        modParticle.SetActive(_mod);
        attackerParticle.SetActive(_attack);
        defenderParticle.SetActive(_defense);
    }

    public void MakeHuge(Vector3 _scale)
    {
        transform.localScale = _scale;
        isHuge = true;
        m_maxHp *= 3.0f;
        m_hp = m_maxHp;
        displayInfo.yOffset = 65.0f;
    }

    public void SetHp(bool _isMod, int _currentLevel)
    {
        m_maxHp = CalculateHP(_currentLevel);
        if (_isMod)
        {
            m_maxHp *= GAMESTATS.modHealthMulti;
        }
        m_hp = m_maxHp;
    }

    private float CalculateHP(int level)
    {
        float baseHp = 10.0f;
        float unModdedHp = Mathf.RoundToInt(baseHp + 0.05f * Mathf.Pow(level, 2));
        return unModdedHp * GAMESTATS.simpHealthMulti;
    }

    public void SetMoney(bool _isMod, int _currentLevel)
    {
        moneyDrop = (ulong)(_currentLevel + (0.1f * Mathf.Pow(_currentLevel, 2)) + (0.001f * Mathf.Pow(_currentLevel, 3)));
        if(_isMod)
        {
            moneyDrop *= 2;
        }
        if(isTransparent || isHuge || attackerParticle.activeSelf || defenderParticle.activeSelf)
        {
            moneyDrop += 5;
        }
    }

    protected virtual void SetRagdollParts()
    {
        m_ragdollParts = new HashSet<Collider>();
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            if (c.gameObject == gameObject || c is MeshCollider)
                continue;

            c.isTrigger = true;

            Rigidbody rigid = c.GetComponent<Rigidbody>();
            if(rigid != null)
            {
                rigid.isKinematic = true;
                rigid.useGravity = false;
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }

            m_ragdollParts.Add(c);
        }
    }

    public void MakeGnome()
    {
        transform.localScale = transform.localScale * 0.6f; // 60% original size
        m_maxHp *= 0.3f; // 30% of max health
        m_hp = m_maxHp;
        isGnome = true;
    }

    public void MakeFriendly()
    {
        isFriendly = true;
        m_target = LevelManager.Inst.GetAliveSimp(transform);
    }

    public void UpdateTarget(Transform _target)
    {
        m_target = _target;
    }

    private void SetBossAffectTransforms(float _scale)
    {
        foreach(var t in bossAffectedTransforms)
        {
            t.localScale = Vector3.one * _scale;
        }
    }

    private void Update()
    {
        OnUpdate();
        if (m_hammerCdr > 0.0f)
        {
            m_hammerCdr -= Time.deltaTime;
        }
        else
        {
            canHammerAttack = true;
        }
    }

    protected virtual void OnUpdate() { }

    public void TakeDamage(float _dmg, Vector3 _throwDir, bool _isTrueDamage = false)
    {
        if (!hasSpawned)
            return;

        if (shieldObject.activeSelf && !_isTrueDamage)
        {
            shieldObject.SetActive(false);
            return;
        }

        float damage = _dmg;
        if(defenderParticle.activeSelf && !_isTrueDamage)
        {
            damage *= 0.5f;
        }

        m_hp -= damage;
        if (isBoss)
        {
            bossHealthBar.SetHealth(m_hp, m_maxHp);
        }
        else
        {
            displayInfo.ShowDamage(damage);
            displayInfo.SetHealth(m_hp / m_maxHp);
        }
        EventManager.onDamageDealt?.Invoke();

        if (m_hp <= 0.0f)
        {
            isDead = true;

            modParticle.SetActive(false);
            attackerParticle.SetActive(false);
            defenderParticle.SetActive(false);
            OnDeath();
            if (isBoss)
                EventManager.onBossBeaten?.Invoke(this);
            else
                EventManager.onSimpBeaten?.Invoke(this);
            displayInfo?.Hide();
            bossHealthBar?.Hide();
            bossHealthBar = null;
            shieldObject.SetActive(false);
            StartCoroutine(TurnIntoRagdoll(_throwDir));
        }
    }

    private void OnDisable()
    {
        EventManager.onSimpSpawned -= OnOtherSimpSpawn;
        EventManager.onSimpBeaten -= OnOtherSimpBeaten;
    }

    private void OnEnable()
    {
        EventManager.onSimpSpawned += OnOtherSimpSpawn;
        EventManager.onSimpBeaten += OnOtherSimpBeaten;
    }

    protected virtual void OnDeath() { }

    [Button]
    public void Test_TurnIntoRagdoll()
    {
        SetRagdollParts();
        StartCoroutine(TurnIntoRagdoll(Vector3.up));
    }

    IEnumerator TurnIntoRagdoll(Vector3 _throwDir)
    {

        foreach (var c in m_ragdollParts)
        {
            c.attachedRigidbody.velocity = Vector3.zero;
            c.attachedRigidbody.angularVelocity = Vector3.zero;
            c.isTrigger = false;
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.useGravity = true;
        }

        if (charController)
            charController.enabled = false;
        m_animator.enabled = false;
        m_navAgent.enabled = false;

        PushBody(_throwDir);
        yield return null;
    }

    public void PushBody(Vector3 _force)
    {
        body.AddForce(_force, ForceMode.Impulse);
    }

    public void PlayAttackParticle(int _index)
    {
        if (attackParticles.Count > _index)
        {
            attackParticles[_index].Play(this);
        }
    }

    public void PlayBanParticle()
    {
        if (canHammerAttack)
        {
            canHammerAttack = false;
            m_banHammerObj.DOScale(Vector3.one * 0.01f, 0.1f).SetEase(Ease.OutBack);
            if (m_banHammerPs.isPlaying)
                m_banHammerPs.Stop();
            m_banHammerPs.Play();
            FMODUnity.RuntimeManager.PlayOneShot(m_hammerSfx);
            m_hammerCdr = 5.0f;
            GameManager.Inst.ExecuteFunctionWithDelay(0.2f, () =>
            {
                Vector3 pos = transform.position + transform.forward * 3.2f;
                var simps = LevelManager.Inst.GetSimpsInRadius(pos, 2.0f);
                foreach(var simp in simps)
                {
                    simp.TakeDamage(GetDamage(), Vector3.up);
                }
            });
        }
    }

    public void StopBanParticle()
    {
        m_banHammerObj.DOScale(Vector3.zero, 0.1f);
    }

    public float GetDamage()
    {
        float min = 1.0f * GAMESTATS.simpDamageMulti;
        float max = 2.0f * GAMESTATS.simpDamageMulti;
        if (isGnome)
        {
            min = max = 1.0f;
        }
        if (isMod)
        {
            min = 2 * GAMESTATS.modDamageMulti;
            max = 3 * GAMESTATS.modDamageMulti;
        }
        if(isBoss)
        {
            min *= 1.5f;
            max *= 1.5f;
        }
        if (attackerParticle.activeSelf)
        {
            min += 1;
            max += 1;
        }
        if(isFriendly)
        {
            min = max = (float)Mathf.RoundToInt(10 + 0.05f * Mathf.Pow(LevelManager.Inst.currentLevel, 2)) / 4.0f;
        }
        float dmg = Random.Range(min, max);
        return dmg;
    }

    private void OnOtherSimpSpawn(Simp _s)
    {
        if(isFriendly && m_target == null)
        {
            m_target = LevelManager.Inst.GetAliveSimp(transform);
        }
    }

    private void OnOtherSimpBeaten(Simp _s)
    {
        if(isFriendly && m_target == _s.transform)
        {
            m_target = LevelManager.Inst.GetAliveSimp(transform);
        }
    }

    public void TryAndUseSkill(string _skill, float _cooldown, UnityAction _skillAction, bool _bypassCld = false)
    {
        if (!m_canUseSkill.ContainsKey(_skill))
            m_canUseSkill.Add(_skill, true);

        if (m_canUseSkill[_skill] || _bypassCld)
        {
            _skillAction.Invoke();
            if (!_bypassCld)
            {
                m_canUseSkill[_skill] = false;
                string skill = _skill;
                GameManager.Inst.ExecuteFunctionWithDelay(_cooldown, () =>
                {
                    if (m_canUseSkill.ContainsKey(skill))
                        m_canUseSkill[skill] = true;
                });
            }
        }
    }

    public void Shoot()
    {
        var obj = PoolManager.Inst.GetObject(EPoolObjectType.SIMP_BULLET);
        SimpBullet bullet = obj.GetComponent<SimpBullet>();
        bullet.Throw(simpBulletTransform.position, dirToTarget.normalized, GetDamage());
    }


    public float throwForce = 450.0f;
    private IEnumerator CO_ThrowSimp()
    {
        if (!isDead)
        {
            float interval = 4.0f;
            bool random = Extensions.ChanceRoll(50.0f);
            if (random)
            {

                var obj = PoolManager.Inst.GetObject(EPoolObjectType.PEPE_RAGDOLL);
                obj.SetActive(true);
                PepeRagdollThrow pepeRagdoll = obj.GetComponent<PepeRagdollThrow>();
                pepeRagdoll.Throw(simpBulletTransform.position + transform.forward * 2.0f + transform.up, dirToTarget, throwForce);

            }
            else
            {
                LevelManager.Inst.SpawnRandomSimp();
                interval += 2.0f;
            }

            yield return new WaitForSeconds(interval);

            StartCoroutine(CO_ThrowSimp());
        }
    }
}
