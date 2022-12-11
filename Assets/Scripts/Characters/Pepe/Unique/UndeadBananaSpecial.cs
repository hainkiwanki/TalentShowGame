using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadBananaSpecial : SimpSpecial
{
    private float m_timeToSpread = 10.0f;

    public override void Cast(string _option)
    {
        m_modSimp.animator.SetBool(EEnemyTransitionParams.usedSpecial.ToString(), true);
        StartCoroutine(CO_SpawnClouds());
    }

    private IEnumerator CO_SpawnClouds()
    {
        int amount = 4;
        float timePerCloud = m_timeToSpread / (float)amount;
        while(amount > 0)
        {
            var go = PoolManager.Inst.GetObject(EPoolObjectType.SMOKE_CLOUD);
            go.SetActive(true);
            go.transform.position = transform.position;
            SmokeCloud cloud = go.GetComponent<SmokeCloud>();
            cloud.StartGrow(15.0f);
            amount--;
            yield return new WaitForSeconds(timePerCloud);
        }

        yield return null;
        m_modSimp.animator.SetBool(EEnemyTransitionParams.usedSpecial.ToString(), false);
    }
}
