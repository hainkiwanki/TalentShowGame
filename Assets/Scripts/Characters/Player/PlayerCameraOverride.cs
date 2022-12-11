using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraOverride : MonoBehaviour
{
    private CharacterControl m_player;
    private Camera m_cam;

    private void Start()
    {
        m_player = FindObjectOfType<CharacterControl>();
        m_cam = GetComponent<Camera>();
        m_player.SetNewCamera(m_cam);
    }

    private void OnDestroy()
    {
        m_player.SetNewCamera(null);        
    }
}
