using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KofuSpecial : SimpSpecial
{
    private Vector2Int m_bunniesAmountInterval = new Vector2Int(1, 10);
    private Vector2 m_scaleInterval = new Vector2(0.3f, 1.5f);
    private Vector2 m_speedInterval = new Vector2(10.0f, 20.0f);

    public override void Cast(string _option)
    {
        if(System.Int32.TryParse(_option, out int result))
        {
            StartCoroutine(CO_SpawnBunnies(result));
        }
    }

    private IEnumerator CO_SpawnBunnies(int _amount)
    {
        int toSpawn = Mathf.Clamp(_amount, m_bunniesAmountInterval.x, m_bunniesAmountInterval.y);
        float amountRatio = (float)toSpawn / (float)m_bunniesAmountInterval.y;
        float scale = Mathf.Lerp(m_scaleInterval.y, m_scaleInterval.x, amountRatio);
        float speed = Mathf.Lerp(m_speedInterval.x, m_speedInterval.y, amountRatio);

        float radius = 1.0f;
        float angle = Random.Range(0.0f, 180.0f);
        Vector3 r = m_modSimp.transform.right;
        Vector3 f = m_modSimp.transform.forward;

        int spawned = 0;
        float spawnInterval = (1.0f / (float)toSpawn);
        while(spawned < toSpawn)
        {
            var bunnyGo = PoolManager.Inst.GetObject(EPoolObjectType.BUNNY);
            bunnyGo.SetActive(true);

            Vector3 pos = transform.position + radius * Mathf.Cos(angle) * r + radius * Mathf.Sin(angle) * f;
            bunnyGo.transform.position = pos;
            Vector3 dir = (m_modSimp.target.position - pos).normalized;
            bunnyGo.transform.rotation = Quaternion.LookRotation(dir);

            Bunny bunny = bunnyGo.GetComponent<Bunny>();
            bunny.BeginMoving(dir, scale, speed, m_modSimp.GetDamage() * spawnInterval);
            spawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

    }
}
