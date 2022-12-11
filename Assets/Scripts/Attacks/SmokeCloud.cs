using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SmokeCloud : PoolObject
{
    private CharacterControl m_player;
    private SphereCollider m_sphereCollider;

    private bool m_isActive = false;
    private float m_radius = 1.7f;
    private float m_growTime = 0.5f;

    protected override void OnTurnOff()
    {
        m_sphereCollider.radius = 0.0f;
        m_isActive = false;
    }


    public void StartGrow(float _seconds)
    {
        if (!m_sphereCollider)
            m_sphereCollider = GetComponent<SphereCollider>();

        m_isActive = true;
        DOTween.To(() => m_sphereCollider.radius, x => m_sphereCollider.radius = x, m_radius, m_growTime);
        StartCoroutine(CO_StartDamaging(_seconds));
    }

    private IEnumerator CO_StartDamaging(float _time)
    {
        float tRemaining = _time;
        float tickTime = 0.05f;
        while(tRemaining > 0.0f)
        {
            if(m_isActive && m_player != null)
            {
                // Damage player
                Debug.Log("Damage player: 0.1 damage");
            }
            tRemaining -= tickTime;
            yield return new WaitForSeconds(tickTime);
        }
        DOTween.To(() => m_sphereCollider.radius, x => m_sphereCollider.radius = x, 0.0f, m_growTime).OnComplete(() =>
        { 
            TurnOff();
        });
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_player = other.gameObject.GetComponent<CharacterControl>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_player = null;
        }
    }
}