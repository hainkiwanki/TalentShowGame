using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePartSystem : MonoBehaviour
{
    public List<ParticleSystem> m_particleSystems = new List<ParticleSystem>();
    private int currentPart = -1;
    private bool isHardRelease = false;
    [FMODUnity.EventRef]
    public string releaseSFX;
    [SerializeField]
    private StudioEventEmitter m_chargingSFX;

    public void Stop(Transform _cam)
    {
        StartCoroutine(DoThrowAnim(_cam));
    }

    public void ForceStop()
    {
        m_chargingSFX.Stop();
        SetActiveParticle(-1);
    }

    IEnumerator DoThrowAnim(Transform _cam)
    {
        m_chargingSFX.Stop();
        SetActiveParticleIndex(-1);

        RuntimeManager.PlayOneShot(releaseSFX, transform.position);

        if (isHardRelease)
        {
            Time.timeScale = 0.0f;
            yield return new WaitForSecondsRealtime(0.05f);
            Time.timeScale = 1.0f;
            _cam.DOShakePosition(0.1f);
            _cam.DOShakeRotation(0.1f, 1.0f);
        }
        isHardRelease = false;
    }

    public void SetPower(float _p) // [ 0 - 1]
    {
        if (!m_chargingSFX.IsPlaying())
            m_chargingSFX.Play();
        float part = _p * (float)m_particleSystems.Count;
        int i = Mathf.FloorToInt(part);
        i = Mathf.Clamp(i, 0, m_particleSystems.Count - 1);

        if (i == m_particleSystems.Count - 1)
            isHardRelease = true;

        SetActiveParticle(i);
        m_chargingSFX.SetParameter("Charge Amount", _p);
    }

    private void SetActiveParticle(int _index)
    {
        if (currentPart != _index)
            SetActiveParticleIndex(_index);
    }

    private void SetActiveParticleIndex(int _index)
    {
        currentPart = _index;
        for (int i = 0; i < m_particleSystems.Count; i++)
        {
            m_particleSystems[i].gameObject.SetActive(i == _index);
        }
    }
}
