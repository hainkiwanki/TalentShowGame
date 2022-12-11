using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Singleton<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
{
    private static T m_instance;

    public static T Inst
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();
            }
            if(m_instance == null)
            {
                var singletonObject = new GameObject();
                m_instance = singletonObject.AddComponent<T>();
                singletonObject.name = typeof(T).ToString();
            }
            return m_instance;
        }
    }
}
