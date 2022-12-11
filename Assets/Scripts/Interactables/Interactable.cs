using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using EPOOutline;

[RequireComponent(typeof(SphereCollider), typeof(Outlinable))]
public class Interactable : SerializedMonoBehaviour
{
    [PropertyOrder(1)]
    public bool useProgression = false;
    [PropertyOrder(1)]
    public bool useSoundEffect = false;

    [HideInInspector]
    public Outlinable outline;
    [HideInInspector]
    public SphereCollider sphereCollider;
    [ReadOnly, HideLabel, HorizontalGroup("GUID"), PropertyOrder(0)]
    public string guid;
    [SerializeField, SuffixLabel("@suffixMessage", true)]
    protected string interactMessage = "";
    protected string suffixMessage = "";

    [PropertyOrder(2)]
    public InteractableData iData;

    public bool canInteract => m_canInteract;   
    protected bool m_canInteract = true;
    protected bool m_isInRange = false;

    [HideInInspector]
    public bool hasLoaded = false;

    [PropertyOrder(2), ShowIf("useProgression")]
    public ProgressionGraph progressGraph;
    private bool isUsingProgression
    {
        get
        {
            return useProgression && progressGraph != null;
        }
    }

    [PropertyOrder(2), ShowIf("useSoundEffect"), FMODUnity.EventRef]
    public string soundEvent;
    private SoundClip sound;
    private bool isUsingSound
    {
        get
        {
            return (useSoundEffect && !string.IsNullOrEmpty(soundEvent));
        }
    }

    private void Awake()
    {
        if(gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
        if (isUsingSound)
            sound = new SoundClip(soundEvent);

        outline = GetComponent<Outlinable>();
        outline.enabled = false;

        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.isTrigger = true;
        }

        OnAwake();
        PreLoad();
    }

    protected virtual void OnDestroy()
    {
        if (isUsingSound)
        {
            sound.Stop();
            sound.Release();
        }
    }

    public virtual void PreLoad()
    {
        if (!SaveData.current.interactableData.ContainsKey(guid))
        {
            SaveData.current.interactableData.Add(guid, new InteractableData() 
            { 
                isToggled = false,
                progress = 0,
            });
        }

        iData = SaveData.current.interactableData[guid];

        ProgressionPreLoad();

        hasLoaded = true;
        m_canInteract = true;
    }

    private void ProgressionPreLoad()
    {
        if (!isUsingProgression)
            return;

        progressGraph.ProgressSilently(iData.progress);
        progressGraph.onProgressComplete += () => { m_canInteract = true; };
    }

    public void Use()
    {
        if (iData.isCompleted)
            return;

        if (isUsingSound)
            sound.Play();
        iData.progress++;
        iData.progress = Mathf.Clamp(iData.progress, 0, 42069);
        OnUse();
        ProgressionOnUse();
    }

    private void ProgressionOnUse()
    {
        if (!isUsingProgression)
            return;

        iData.progress = progressGraph.Progress(iData.progress);
        m_canInteract = false;

        if (progressGraph.isDone)
        {
            UIManager.Inst.notifications.ShowNotification("Completed interaction");
            iData.isCompleted = true;
        }
    }

    public void Highlight()
    {
        OnEnter();
        outline.enabled = true;
    }

    public void Dim()
    {
        OnExit();
        outline.enabled = false;
    }

    #region Strict_Virtuals
    protected virtual void OnAwake() { }

    protected virtual void OnUse() { }

    protected virtual void OnEnter() { }

    protected virtual void OnExit() { }
    #endregion

    public virtual void Continue() 
    {
        if (isUsingProgression)
            progressGraph.Continue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !iData.isCompleted)
        {
            CharacterControl control = other.GetComponent<CharacterControl>();
            control.EnterInteractable(this);
            m_isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !iData.isCompleted)
        {
            CharacterControl control = other.GetComponent<CharacterControl>();
            control.ExitInteractable(this);
            m_isInRange = false;
        }
    }

    public virtual string GetInteractMessage()
    {
        return interactMessage + suffixMessage;
    }

    [Button, HorizontalGroup("GUID")]
    public void NewGUID()
    {
        guid = GameManager.Inst.GenerateGUID();
    }
}
