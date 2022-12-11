using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

[System.Serializable]
public class ModPowerBannerInfo
{
    public string modName;
    public Color color = Color.white;
    public Sprite modImage;
    public Sprite modWhiteImage;
}

public enum EModNames
{
    Kofu = 0,
    Slyde = 1,
    Undeadbanana = 2,
    Separatedice = 3,
    Nykz = 4,
}

public class ModPowerBanner : MonoBehaviour
{
    [SerializeField]
    private Image m_japaneseCharactersImage;
    [SerializeField]
    private TextMeshProUGUI m_modNameText;
    [SerializeField]
    private Image m_modBannerColorImage;

    [SerializeField]
    private Image m_modImage, m_modWhiteImage;
    [SerializeField]
    private Animator m_animator;

    public List<ModPowerBannerInfo> modPowerData = new List<ModPowerBannerInfo>();
    public List<Sprite> japaneseSprites = new List<Sprite>();

    [FMODUnity.EventRef, SerializeField]
    private string m_japaneseSoundEffect, m_enterSoundEffect;

    private List<EModNames> m_queue = new List<EModNames>();
    private List<UnityAction> m_queueActions = new List<UnityAction>();

    public void PlayAnimation(EModNames _name, UnityAction _action)
    {
        if (gameObject.activeSelf)
        {
            m_queue.Add(_name);
            m_queueActions.Add(_action);
        }
        else
            ForcePlay(_name, _action);
    }

    private void ForcePlay(EModNames _name, UnityAction _action)
    {
        ModPowerBannerInfo info = null;
        info = modPowerData[(int)_name];
        if (info != null)
        {
            gameObject.SetActive(true);
            StartCoroutine(CO_PlayAnimation(info, _action));
        }
    }

    IEnumerator CO_PlayAnimation(ModPowerBannerInfo _info, UnityAction _action)
    {
        FMODUnity.RuntimeManager.PlayOneShot(m_enterSoundEffect);
        m_japaneseCharactersImage.sprite = japaneseSprites.GetRandom();
        m_modNameText.text = _info.modName;
        m_modBannerColorImage.color = _info.color;
        m_modImage.sprite = _info.modImage;
        m_modWhiteImage.sprite = _info.modWhiteImage;
        m_japaneseCharactersImage.transform.DOShakePosition(6.5f, 14.0f, 40).SetUpdate(true);

        yield return new WaitForSecondsRealtime(1.0f);

        Time.timeScale = 0.0f;
        FMODUnity.RuntimeManager.PlayOneShot(m_japaneseSoundEffect);
        _action.Invoke();

        yield return new WaitForSecondsRealtime(3.5f);

        Time.timeScale = 1.0f;
        gameObject.SetActive(false);

        if(m_queue.Count > 0)
        {
            ForcePlay(m_queue[0], m_queueActions[0]);
            m_queue.RemoveAt(0);
            m_queueActions.RemoveAt(0);
        }
    }
}
