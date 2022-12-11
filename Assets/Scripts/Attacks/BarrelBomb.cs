using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBomb : PoolObject
{
    private SphereCollider collider;
    private float m_maxRadius = 6.0f;
    public void Initialize(Vector3 _position)
    {
        if (collider == null)
            collider = GetComponent<SphereCollider>();
        transform.position = _position;
        collider.radius = 0.0f;
        DOTween.To(() => collider.radius, x => collider.radius = x, m_maxRadius, 2.0f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") || other.CompareTag("PlayerParts"))
        {
            var player = GameManager.Inst.playerControl;
            float distance = Vector3.Distance(player.transform.position, transform.position);
            float ratio = distance / m_maxRadius;
            int damage = Mathf.CeilToInt(Mathf.Lerp(1.0f, 4.0f, 1 - ratio));
            Debug.Log(damage);
            player.TakeDamage((float)damage);
        }
    }
}
