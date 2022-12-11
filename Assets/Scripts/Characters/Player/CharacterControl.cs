using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterControl : SerializedMonoBehaviour
{
    private List<Collider> m_ragdollParts = new List<Collider>();
    [SerializeField]
    private Animator m_animator;

    public Vector3 input = Vector3.zero;
    public Vector3 moveDir
    {
        get
        {
            return MoveDirection(input);
        }
    }
    public Vector3 mousePos = Vector3.zero;
    public CharacterController controller
    {
        get
        {
            if (m_controller == null)
                m_controller = GetComponent<CharacterController>();
            return m_controller;
        }
    }
    private CharacterController m_controller;
    public bool usedPrimary = false;
    public bool usedSecondary = false;
    public bool isChargingPrimary = false;
    public bool usedDodge = false;
    private PlayerComposer playerComposer;
    private Camera m_cam, m_sceneCam, m_mainCam;
    public Camera camera => m_cam;

    private Interactable m_currentInteractable;
    private Interactable m_previousInteractable;
    private ManualInput m_manualImput;

    public SphereCollider groundDetector;
    public ParticlePartSystem chargeParticle;
    private PlayerInfoWindow playerUI;

    [HideInInspector]
    public bool isImmune = false;
    [HideInInspector]
    public bool isDead = false;
    private float health;
    private float maxHealth;
    public EPoolObjectType currentNormalAmmo = EPoolObjectType.STEAK;
    public EPoolObjectType currentChargedAmmo = EPoolObjectType.CHARGED_STEAK;

    private Avatar m_animAvatar;
    public int slowStacks;
    private bool m_hasBeenSlowed = false;
    private float m_slowTimer = 0.0f;

    [FMODUnity.EventRef, SerializeField]
    private string m_curseSfx, m_ayayaSfx;

    public void Initialize()
    {
        EventManager.onSimpBeaten += OnSimpDead;
        EventManager.onPlayerStatModiferAdded += OnUpgradePicked;
        EventManager.onSceneLoaded += (int i) => { UpdatePlayerVisuals(); };

        GameManager.Inst.uicontrols.UI.Interrupt.performed += _ => InteractableContinue();

        playerUI = (PlayerInfoWindow)UIManager.Inst.uiWindows[EUiWindows.PlayerInformation];
        SetRagdollParts();
        playerComposer = GetComponent<PlayerComposer>();
        m_cam = m_mainCam = Camera.main;
        m_manualImput = GetComponent<ManualInput>();
        m_animAvatar = m_animator.avatar;
    }

    public void UpdatePlayerVisuals()
    {
        playerComposer.UpdateColors();
        playerComposer.UpdateParts();
    }

    public void PreRunInit()
    {
        health = maxHealth = GAMESTATS.maxHealth;
        if (GAMESTATS.isJebaited)
            health = 1.0f;
    }

    public void PreLevelInit()
    {
        playerUI.ShowCombatUI();
        playerUI.SetHealth(health, maxHealth);
        isDead = false;
    }

    public bool IsGrounded()
    {
        var cols = Physics.OverlapSphere(groundDetector.transform.position, groundDetector.radius, 1 << 8);
        if (cols.Length > 0)
            return true;

        return false;
    }

    public void SetPosition(Vector3 _pos)
    {
        transform.position = _pos;
    }

    public void SetNewCamera(Camera _cam)
    {
        m_sceneCam = _cam;
        if (m_sceneCam == null)
        {
            m_cam = m_mainCam;
        }
        else
        {
            m_cam = m_sceneCam;
        }
        m_mainCam.gameObject.SetActive(m_sceneCam == null);
        m_manualImput.SetCam(m_cam);
    }

    #region Movement_Logic
    public void Move(Vector3 _moveDir, float _speed)
    {
        m_controller.Move(_moveDir * _speed * Time.deltaTime);
    }

    public void ForceMove(Vector3 _dir, float _speed)
    {
        m_controller.Move(_dir * _speed * Time.deltaTime);
    }

    public void Rotate(Vector3 _lookDir, float _speed)
    {
        if (_lookDir == Vector3.zero)
            return;

        Quaternion lookRot = Quaternion.LookRotation(_lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, _speed * Time.deltaTime);
    }

    private Vector3 MoveDirection(Vector3 _inputDirection)
    {
        Vector3 f = m_cam.transform.forward.NewY(0.0f).normalized;
        Vector3 r = m_cam.transform.right.NewY(0.0f).normalized;
        Vector3 direction = (_inputDirection.z * f + _inputDirection.x * r).normalized;
        return direction;
    }
    #endregion

    #region Ragdoll
    private void SetRagdollParts()
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            if (c.gameObject == gameObject)
                continue;
            c.isTrigger = true;
            m_ragdollParts.Add(c);
        }
    }

    public void TurnOnRagdoll()
    {
        controller.enabled = false;
        m_animator.enabled = false;
        m_animator.avatar = null;
        foreach (Collider c in m_ragdollParts)
        {
            if(c.attachedRigidbody != null)
                c.attachedRigidbody.velocity = Vector3.zero;
            c.isTrigger = false;
            if (c.name == "Hips")
            {
                c.attachedRigidbody.AddForce(-transform.forward * 100.0f, ForceMode.Impulse);
            }
        }
    }

    public void TurnOffRagdoll()
    {
        controller.enabled = true;
        m_animator.enabled = true;
        m_animator.avatar = m_animAvatar;
        foreach (Collider c in m_ragdollParts)
        {
            c.isTrigger = true;
        }
    }
    #endregion

    #region Interactables
    public void EnterInteractable(Interactable _interactable)
    {
        if (m_currentInteractable != null && m_currentInteractable != _interactable)
            m_currentInteractable.Dim();

        m_currentInteractable = _interactable;

        if(m_currentInteractable != null)
        {
            m_currentInteractable.Highlight();
            UIManager.Inst.interactMessage.ShowInteract(m_currentInteractable.GetInteractMessage());
        }
    }

    public void ExitInteractable(Interactable _interactable)
    {
        if(m_currentInteractable)
            m_currentInteractable.Dim();
        m_previousInteractable = m_currentInteractable;
        if (m_currentInteractable != null && m_currentInteractable == _interactable)
        {
            m_currentInteractable = null;
            UIManager.Inst.interactMessage.Hide();
        }
    }

    public void Interact()
    {
        if(m_currentInteractable != null && m_currentInteractable.canInteract)
        {
            m_currentInteractable.Use();
            UIManager.Inst.interactMessage.SetInteractMessage(m_currentInteractable.GetInteractMessage());
        }
    }

    private void InteractableContinue()
    {
        if (m_currentInteractable != null)
        {
            m_currentInteractable.Continue();
        }
        else if(m_previousInteractable != null)
        {
            m_previousInteractable.Continue();
        }
    }
    #endregion

    public bool CanAttack()
    {
        return !isDead && SceneLoader.Inst.currentSceneIndex == 4;
    }
  
    public void TakeDamage(float _damage)
    {
        if (isImmune)
            return;

        health -= _damage * GAMESTATS.damageTakenMulti;
        health = Mathf.Clamp(health, 0.0f, maxHealth);
        playerUI.SetHealth(health, maxHealth);

        if(health <= 0.0f)
        {
            LevelManager.Inst.OnDefeat();
            isDead = true;
            TurnOnRagdoll();
        }
    }

    private void OnSimpDead(Simp _s)
    {
        ReceiveMoney(_s.moneyDrop);
        if(GAMESTATS.lifeSteal > 0)
        {
            health += GAMESTATS.lifeSteal;
            health = Mathf.Clamp(health, 0.0f, maxHealth);
            playerUI.SetHealth(health, maxHealth);
        }
    }

    private void ReceiveMoney(float _f)
    {
        SaveData.current.playerMoney += (ulong)Mathf.CeilToInt(_f * GAMESTATS.moneyMulti);
        playerUI.SetMoney(SaveData.current.playerMoney);
    }

    public void Heal(float _hp)
    {
        float heal = Mathf.Clamp(_hp, health, maxHealth);
        health = heal;
        playerUI.SetHealth(health, maxHealth, true);
        playerUI.SetMoney(SaveData.current.playerMoney, true);
    }

    private void OnUpgradePicked(PlayerModifier _modifer)
    {
        switch (_modifer.stat)
        {
            case ECharacterStat.MOVEMENTSPEED:
                Debug.Log("Gained movement speed");
                m_animator.SetFloat(EPlayerTransitionParams.moveSpeed.ToString(), GAMESTATS.moveMulti);
                break;
            case ECharacterStat.DAMAGE_DEALT:
                Debug.Log("Damage changed");
                break;
            case ECharacterStat.DAMAGE_TAKEN_MULTIPLIER:
                Debug.Log("Damage taken changed");
                break;
            case ECharacterStat.MAXHEALTH:
                Debug.Log("Gained max health");
                health += _modifer.modifer.amt;
                maxHealth = GAMESTATS.maxHealth;
                playerUI.SetHealth(health, maxHealth, true);
                break;
            case ECharacterStat.LIFESTEAL:
                Debug.Log("Lifesteal changed");
                break;
            case ECharacterStat.GOLD_MULTIPLIER:
                Debug.Log("Gold multiplier changed");
                break;
            case ECharacterStat.BULLET_SIZE:
                Debug.Log("Bullet size changed");
                break;
            case ECharacterStat.CHARGE_TIME:
                Debug.Log("Charge time changed");
                break;
            case ECharacterStat.ATTACKSPEED:
                Debug.Log("Gained attack speed");
                m_animator.SetFloat(EPlayerTransitionParams.attackSpeed.ToString(), GAMESTATS.attackSpeed);
                break;
            default:
                break;
        }
    }

    public void AddSlowProjectTile()
    {
        slowStacks++;
        slowStacks = Mathf.Clamp(slowStacks, 0, 10);
        m_slowTimer = 5.0f;
        if (!m_hasBeenSlowed)
            StartCoroutine(CO_RemoveSlow());
    }

    public bool DodgeWhenSlowed()
    {
        if(Extensions.ChanceRoll(75.0f))
        {
            slowStacks--;
        }
        return slowStacks <= 0;
    }

    private IEnumerator CO_RemoveSlow()
    {
        m_hasBeenSlowed = true;
        UIManager.Inst.notifications.ShowNotification("You have been slowed");
        while(m_slowTimer > 0.0f)
        {
            m_slowTimer -= Time.deltaTime;
            yield return null;
        }
        slowStacks = 0;
        m_hasBeenSlowed = false;
        UIManager.Inst.notifications.ShowNotification("Slow lifted");
    }

    public void OnAttack()
    {
        if (Extensions.ChanceRoll(80.0f))
        {
            if (GAMESTATS.canAyaya && GAMESTATS.canCurse)
            {
                if (Extensions.ChanceRoll(50.0f))
                {
                    FMODUnity.RuntimeManager.PlayOneShot(m_ayayaSfx);
                }
                else
                {
                    FMODUnity.RuntimeManager.PlayOneShot(m_curseSfx);
                }
            }
            else if (GAMESTATS.canCurse)
            {
                FMODUnity.RuntimeManager.PlayOneShot(m_curseSfx);
            }
            else if (GAMESTATS.canAyaya)
            {
                FMODUnity.RuntimeManager.PlayOneShot(m_ayayaSfx);
            }
        }
    }
}
