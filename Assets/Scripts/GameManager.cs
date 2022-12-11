using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public TwitchClient client;

    [HideInInspector]
    public CameraControl playerCamera;
    [HideInInspector]
    public CharacterControl playerControl;

    public GameStats gameStats;
    private FMOD.Studio.Bus musicBus;

    [SerializeField]
    private RunModifierWindow m_runModWindow;

    private void Awake()
    {
        playerCamera = FindObjectOfType<CameraControl>();
        playerControl = FindObjectOfType<CharacterControl>();
        playerControl.Initialize();
        SceneLoader.Inst.LoadScene(ESceneIndices.Menu, () => 
        {
            if(SerializationManager.HasSave())
                playerControl.UpdatePlayerVisuals();
        });
        PlayerInfoWindow playerInfo = (PlayerInfoWindow)UIManager.Inst.uiWindows[EUiWindows.PlayerInformation];
        playerInfo.SetMoney(SaveData.current.playerMoney);
    }

    private void Start()
    {
        GameManager.Inst.uicontrols.UI.Pause.performed += _ => TogglePause();
        gameStats.Initialize(SaveData.current);
        m_runModWindow.Intitialize(SaveData.current);
        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
    }
    private void OnEnable()
    {
        uicontrols.Enable();
        controls.Enable();
    }

    private void OnDisable()
    {
        uicontrols.Disable();
        controls.Disable();
    }

    public void ChangeMusicVolume(float _volume)
    {
        musicBus.setVolume(_volume);
    }

    private void TogglePause()
    {
        if(SceneLoader.Inst.currentSceneIndex == (int)ESceneIndices.Game ||
            SceneLoader.Inst.currentSceneIndex == (int)ESceneIndices.Home)
        {
            UIManager.Inst.uiWindows[EUiWindows.Pause].Toggle();
        }
    }

    public void LoadGameScene()
    {
        OnSave();
        SceneLoader.Inst.LoadScene(ESceneIndices.Game);
    }

    [Button]
    public void DeleteSave()
    {
        SerializationManager.DeleteSave();
    }

    [Button]
    public void OnSave()
    {
        SerializationManager.Save(SaveData.current);
    }

    public void OnMenu()
    {
        SceneLoader.Inst.LoadScene(ESceneIndices.Menu);
        UIManager.Inst.warningMessage.ShowMessage("Saved", 1.0f);
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void SetPlayerCamera(bool _turnOn = true)
    {
        playerCamera.gameObject.SetActive(_turnOn);
    }

    public void SetPlayerControl(bool _enable = true)
    {
        playerControl.gameObject.SetActive(_enable);
    }

    public void ResetPlayer()
    {
        playerControl.SetPosition(Vector3.zero);
        playerControl.TurnOffRagdoll();
    }

    public Camera GetActiveCam()
    {
        return playerControl.camera;
    }

    #region Controls
    public PlayerInput controls
    {
        get
        {
            if (_controls == null)
            {
                _controls = new PlayerInput();
            }

            return _controls;
        }
    }
    private PlayerInput _controls;

    public UIInput uicontrols
    {
        get
        {
            if (_uicontrols == null)
                _uicontrols = new UIInput();

            return _uicontrols;
        }
    }
    private UIInput _uicontrols;
    #endregion

    public void ExecuteFunctionWithDelay(float _delay, UnityAction _function)
    {
        StartCoroutine(CO_ExecuteAfterSeconds(_delay, _function));
    }

    IEnumerator CO_ExecuteAfterSeconds(float _t, UnityAction _executeAfter)
    {
        yield return new WaitForSeconds(_t);
        _executeAfter?.Invoke();
    }

    public void ExecuteFunctionWithDelayRealtime(float _delay, UnityAction _function)
    {
        StartCoroutine(CO_ExecuteAfterSecondsRealtime(_delay, _function));
    }

    IEnumerator CO_ExecuteAfterSecondsRealtime(float _t, UnityAction _f)
    {
        yield return new WaitForSecondsRealtime(_t);
        _f?.Invoke();
    }

    public string GenerateGUID()
    {
        return Guid.NewGuid().ToString("N");
    }
}
