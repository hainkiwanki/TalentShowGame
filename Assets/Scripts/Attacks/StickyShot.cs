using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyShot : PoolObject
{
    private bool m_hasThrown = false;
    private Vector3 m_throwDirection;
    private float m_speed;
    private bool m_isStuck = false;
    [SerializeField]
    private List<GameObject> m_particles = new List<GameObject>();
    [SerializeField, FMODUnity.EventRef]
    private string m_popSfx, m_stickSfx;

    public void Throw(Vector3 _direction, float _speed)
    {
        FMODUnity.RuntimeManager.PlayOneShot(m_popSfx);
        m_throwDirection = _direction;
        m_speed = _speed;
        m_isStuck = false;
        m_hasThrown = true;
        transform.parent = null;
        foreach (var particle in m_particles)
            particle.SetActive(true);
    }

    private void Update()
    {
        if(m_hasThrown && !m_isStuck)
        {
            if(transform.parent == null)
                transform.position += m_throwDirection * m_speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_isStuck)
            return;

        if(other.gameObject != gameObject && !other.CompareTag("Pepe") && !other.CompareTag("PepeParts") && !other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<StickyShot>() != null)
                return;

            FMODUnity.RuntimeManager.PlayOneShot(m_stickSfx);
            m_isStuck = true;

            foreach (var particle in m_particles)
                particle.SetActive(false);

            if(other.CompareTag("PlayerParts") || other.CompareTag("Player"))
            {
                var player = GameManager.Inst.playerControl;
                player.AddSlowProjectTile();
            }

            TurnOff();
        }
    }

    protected override void OnTurnOff()
    {
        m_hasThrown = false;
        transform.parent = null;
    }
}
