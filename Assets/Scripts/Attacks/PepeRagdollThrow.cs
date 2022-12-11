using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepeRagdollThrow : PoolObject
{
    [SerializeField]
    private Rigidbody m_rigidBody;
    private bool m_hasThrown = false;
    [SerializeField]
    private SphereCollider m_collider;

    public void Throw(Vector3 _pos, Vector3 _dir, float _force)
    {
        m_collider.enabled = true;
        transform.position = _pos;
        transform.rotation = Quaternion.identity;
        m_rigidBody.AddForce(_dir * _force, ForceMode.Impulse);
        GameManager.Inst.ExecuteFunctionWithDelay(0.1f, () => { m_hasThrown = true; });
    }

    private void Update()
    {
        if(m_hasThrown)
        {
            if(m_rigidBody.velocity.magnitude <= 0.1f)
            {
                m_collider.enabled = false;
                m_hasThrown = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.CompareTag("Player") || other.CompareTag("PlayerParts")) && m_hasThrown)
        {
            GameManager.Inst.playerControl.TakeDamage(2.0f);
        }
    }
}
