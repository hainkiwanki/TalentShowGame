using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : PoolObject
{
    [SerializeField]
    private List<Transform> m_scalableParts = new List<Transform>();

    private Vector3 m_direction;
    private bool m_hasDirection = false;
    private float m_speed;
    private float m_dmg;

    public void BeginMoving(Vector3 _direction, float _scale, float _speed, float _dmg)
    {
        foreach (var transform in m_scalableParts)
            transform.localScale = Vector3.one * _scale;
        m_speed = _speed;
        m_direction = _direction;
        m_hasDirection = true;
        m_dmg = _dmg;
    }

    private void Update()
    {
        if(m_hasDirection)
            transform.position += m_direction * m_speed * Time.deltaTime;
    }

    protected override void OnTurnOff()
    {
        m_hasDirection = false;
        m_direction = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") || other.CompareTag("PlayerParts"))
        {
            var player = GameManager.Inst.playerControl;
            player.TakeDamage(m_dmg);
        }
    }
}