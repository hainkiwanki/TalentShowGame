using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlydeSpecial : SimpSpecial
{
    public float radius = 0.2f;
    public float speed = 25.0f;

    public override void Cast(string _option)
    {
        if (System.Int32.TryParse(_option, out int result))
        {
            StartCoroutine(CO_FireShots(result));
        }
    }

    private IEnumerator CO_FireShots(int _amountToFire)
    {
        int toFire = Mathf.Clamp(_amountToFire, 10, 50);
        int amount = 0;

        float interval = 3.0f / (float)toFire;

        while(amount < toFire)
        {
            var obj = PoolManager.Inst.GetObject(EPoolObjectType.STICKY_SHOT);
            obj.SetActive(true);
            StickyShot shot = obj.GetComponent<StickyShot>();

            Vector3 spawnPos = GetRandomStickShotPos(m_modSimp.simpBulletTransform);
            shot.transform.position = spawnPos;
            shot.transform.rotation = Quaternion.LookRotation(m_modSimp.dirToTarget);
            shot.Throw(m_modSimp.dirToTarget, speed);
            amount++;
            yield return new WaitForSeconds(interval);
        }
    }

    private Vector3 GetRandomStickShotPos(Transform _transform)
    {
        return GetRandomStickShotPos(_transform.up, _transform.right, _transform.position);
    }

    private Vector3 GetRandomStickShotPos(Vector3 _up, Vector3 _right, Vector3 _center)
    {
        float angle = Random.Range(0.0f, 360.0f);
        return _center + radius * Mathf.Cos(angle) * _right + radius * Mathf.Sin(angle) * _up;
    }
}
