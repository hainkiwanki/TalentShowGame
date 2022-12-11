using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSimp : Simp
{
    public SkinnedMeshRenderer meshRenderer;
    public GameObject maskObject;
    private float maskHeight = 2.2f;
    private Material m_targetMaterial, m_spawnMaterial;

    protected override void OnSimpInitialize()
    {
        maskHeight = 2.2f;
        MoveMaskPlane();
    }

    protected override void OnBossInitialize()
    {
        maskHeight = 4.4f;
        MoveMaskPlane();
    }

    private void MoveMaskPlane()
    {
        maskObject.transform.DOMoveY(maskHeight, 2.0f).SetDelay(0.45f).OnComplete(() =>
        {
            hasSpawned = true;
            m_animator.Play("Jog 0");
            meshRenderer.material = m_targetMaterial;
            if (!isTransparent)
                displayInfo.Show();
        });
    }

    public void SetMaterials(Material _spawnMaterial, Material _playMaterial, bool _shadows = true)
    {
        m_spawnMaterial = _spawnMaterial;
        m_targetMaterial = _playMaterial;
        meshRenderer.shadowCastingMode = (_shadows) ? UnityEngine.Rendering.ShadowCastingMode.On :
            UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.material = m_spawnMaterial;
    }

    protected override void OnUpdate()
    {
        if ((!hasSpawned || isDead) && maskObject != null && m_spawnMaterial != null)
        {
            m_spawnMaterial.SetVector("_DissolveMaskPosition", maskObject.transform.position);
            m_spawnMaterial.SetVector("_DissolveMaskNormal", maskObject.transform.up);
        }
    }

    protected override void OnDeath()
    {
        meshRenderer.material = m_spawnMaterial;
        maskObject.transform.DOMoveY(0.0f, 2.0f).SetDelay(5.0f).OnComplete(() =>
        {
            TurnOff();
        });
    }
}
