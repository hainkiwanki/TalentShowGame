using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpBullet : PoolObject
{
    private float m_damage;
    private bool m_canMove = false;
    private float m_speed = 10.0f;

    public void Throw(Vector3 _startPos, Vector3 _dir, float _dmg)
    {
        transform.position = _startPos;
        transform.rotation = Quaternion.LookRotation(_dir);
        m_damage = _dmg;
        m_canMove = true;
    }

    private void Update()
    {
        if(m_canMove)
        {
            transform.position += transform.forward * m_speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerParts") || other.CompareTag("Player"))
        {
            var player = GameManager.Inst.playerControl;
            player.TakeDamage(m_damage);
            m_canMove = false;
            TurnOff();
        }
    }
}
