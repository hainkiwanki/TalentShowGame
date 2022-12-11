using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : PoolObject
{
    public List<GameObject> particleTiers = new List<GameObject>();
    private Rigidbody rigidBody;
    [HideInInspector]
    public float damage = 0.0f;
    [EventRef, SerializeField]
    private string impactSFX;
    [EventRef, SerializeField]
    private string fireSFX;
    private SphereCollider trigger;

    public Vector3 correctionRotation = Vector3.zero;
    public int hitsLeft = 1;
    private int countedHits = 0;

    private List<Simp> simpsHit;

    private MeshRenderer meshRenderer;
    private Material material;

    [SerializeField]
    private List<Transform> m_particleTransforms = new List<Transform>();

    private void Awake()
    {
        trigger = GetComponent<SphereCollider>();
    }

    public void AddForce(Vector3 _force, int _tier)
    {
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            material = meshRenderer.sharedMaterial;
        }
         material.SetFloat("_FoodSize", GAMESTATS.bulletSize);
        foreach (var ps in m_particleTransforms)
            ps.localScale = Vector3.one * GAMESTATS.bulletSize;

        countedHits = hitsLeft;
        simpsHit = new List<Simp>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce(_force, ForceMode.Impulse);
        rigidBody.AddTorque(_force + transform.right);
        RuntimeManager.PlayOneShot(fireSFX);

        trigger.enabled = true;

        foreach (var ps in particleTiers)
            ps.SetActive(false);

        if (_tier < particleTiers.Count && _tier >= 0)
        {
            particleTiers[_tier].SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != gameObject && other.CompareTag("Pepe"))
        {
            Simp simp = other.GetComponent<Simp>();
            if (simp != null && rigidBody.velocity.magnitude > 1.0f)
            {
                if (simpsHit.Contains(simp))
                    return;

                simpsHit.Add(simp);
                RuntimeManager.PlayOneShot(impactSFX);
                if (countedHits <= 0)
                    return;

                Vector3 force = rigidBody.velocity.NewY(0.0f) * 10.0f;
                simp.TakeDamage(damage, force);
                trigger.enabled = false;
                foreach (var ps in particleTiers)
                    ps.SetActive(false);
                countedHits--;
            }
        }
        else if(!other.CompareTag("Player"))
        {
            trigger.enabled = false;
            foreach (var ps in particleTiers)
                ps.SetActive(false);
            countedHits--;
        }
    }
}