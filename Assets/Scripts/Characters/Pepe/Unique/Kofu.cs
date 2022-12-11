using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kofu : ModSimp
{
    [SerializeField]
    private GameObject m_kofuModel;

    protected override void SetRagdollParts()
    {
        base.SetRagdollParts();
        foreach (var collider in m_ragdollParts)
        {
            if (collider.CompareTag("PepeParts"))
            {
                collider.attachedRigidbody.velocity = Vector3.zero;
                collider.attachedRigidbody.angularVelocity = Vector3.zero;
                collider.isTrigger = false;
                collider.attachedRigidbody.isKinematic = false;
                collider.attachedRigidbody.useGravity = true;
            }
            else
            {
                collider.isTrigger = false;
            }
        }
    }

    protected override void OnSimpInitialize()
    {
        base.OnSimpInitialize();
        m_kofuModel.SetActive(false);
        GameManager.Inst.ExecuteFunctionWithDelay(1.5f, () =>
        { 
            m_kofuModel.SetActive(true);
        });
    }
}
