using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PoolObject : SerializedMonoBehaviour
{
    public EPoolObjectType poolObjectType;
    public float scheduleOffTime;
    private Coroutine offRoutine;

    private void OnEnable()
    {
        if (offRoutine != null)
        {
            StopCoroutine(offRoutine);
        }

        if (scheduleOffTime != -1 && scheduleOffTime > 0.0f)
            offRoutine = StartCoroutine(ScheduleOff());
    }

    public void TurnOff()
    {
        OnTurnOff();
        PoolManager.Inst.AddObject(this);
    }

    protected virtual void OnTurnOff()
    { }

    IEnumerator ScheduleOff()
    {
        yield return new WaitForSeconds(scheduleOffTime);

        if (!PoolManager.Inst.poolDictionary[poolObjectType].Contains(gameObject))
        {
            TurnOff();
        }
    }
}