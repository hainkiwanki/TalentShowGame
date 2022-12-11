using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpAttack : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;

    private Simp m_simp;
    private int m_hitsLeft = 1;

    public void Play(Simp _simp)
    {
        m_simp = _simp;
        m_hitsLeft = 1;
        if (ps.isPlaying)
            ps.Stop();
        ps.Play();
    }

    private void Update()
    {
        if(ps.isPlaying && m_simp != null && m_hitsLeft > 0)
        {
            Ray ray = new Ray(m_simp.transform.position + Vector3.up * 0.8f, m_simp.transform.forward);
            if (Physics.SphereCast(ray, 0.5f, out RaycastHit hitinfo, 1.0f, 1 << 9))
            {
                CharacterControl player = hitinfo.collider.gameObject.GetComponent<CharacterControl>();
                if (player != null)
                {
                    m_hitsLeft--;
                    player.TakeDamage(m_simp.GetDamage());
                }
            }
        }
    }
}
