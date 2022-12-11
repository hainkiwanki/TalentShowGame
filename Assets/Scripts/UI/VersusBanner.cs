using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersusBanner : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_simpName;
    [SerializeField, FMODUnity.EventRef]
    private string vsTextEffect, vsPopUpEffect;

    public void Show(string _simpName)
    {
        m_simpName.text = _simpName;
        gameObject.SetActive(true);
        StartCoroutine(CO_Show());
    }

    IEnumerator CO_Show()
    {
        Time.timeScale = 0.0f;
        FMODUnity.RuntimeManager.PlayOneShot(vsPopUpEffect);
        yield return new WaitForSecondsRealtime(0.4f);
        FMODUnity.RuntimeManager.PlayOneShot(vsTextEffect);
        yield return new WaitForSecondsRealtime(5.6f);
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
    }
}
