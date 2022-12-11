using UnityEngine;

public class PoolObjectLoader : MonoBehaviour
{
    public static PoolObject InstantiatePrefab(EPoolObjectType _objType)
    {
        GameObject obj = null;

        switch (_objType)
        {
            case EPoolObjectType.STEAK:
                obj = Instantiate(Resources.Load<GameObject>("Food/Steak_Cooked"));
                break;
            case EPoolObjectType.SIMP:
                obj = Instantiate(Resources.Load<GameObject>("Pepes/Pepe"));
                break;
            case EPoolObjectType.SIMPSTATS:
                obj = Instantiate(Resources.Load<GameObject>("Pepes/SimpDisplayInfo"));
                break;
            case EPoolObjectType.CHARGED_STEAK:
                obj = Instantiate(Resources.Load<GameObject>("Food/Charged_Steak"));
                break;
            case EPoolObjectType.SIMP_BULLET:
                obj = Instantiate(Resources.Load<GameObject>("SimpBullet"));
                break;
            case EPoolObjectType.STICKY_SHOT:
                obj = Instantiate(Resources.Load<GameObject>("StickyShot"));
                break;
            case EPoolObjectType.SMOKE_CLOUD:
                obj = Instantiate(Resources.Load<GameObject>("SmokeCloud"));
                break;
            case EPoolObjectType.BUNNY:
                obj = Instantiate(Resources.Load<GameObject>("RabbitCharge"));
                break;
            case EPoolObjectType.BARREL_BOMB:
                obj = Instantiate(Resources.Load<GameObject>("BarrelExplosion"));
                break;
            case EPoolObjectType.PEPE_RAGDOLL:
                obj = Instantiate(Resources.Load<GameObject>("Pepes/PepeBodyThrow"));
                break;
            default:
                break;
        }

        return obj.GetComponent<PoolObject>();
    }
}
