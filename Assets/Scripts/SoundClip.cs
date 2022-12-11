using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClip
{
    private FMOD.Studio.EventInstance m_eventInstance;
    private FMOD.Studio.EventDescription m_eventDescription;
    private int m_length;

    public bool isValid => m_eventInstance.isValid();

    public float duration => (float)m_length / 1000.0f;

    public SoundClip(string _eventPath)
    {
        m_eventDescription = RuntimeManager.GetEventDescription(_eventPath);
        if (!m_eventDescription.isValid())
        {
            Debug.LogError("Not a valid sound event path");
        }
        else
        {
            m_eventDescription.createInstance(out m_eventInstance);
            m_eventDescription.getLength(out m_length);
        }
    }

    public void Play()
    {
        if(m_eventInstance.isValid())
        {
            m_eventInstance.start();
        }
    }

    public void Stop(bool _fadeOut = false)
    {
        if(m_eventInstance.isValid())
        {
            m_eventInstance.stop((_fadeOut) ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void Release()
    {
        if(m_eventInstance.isValid())
        {
            m_eventInstance.release();
            m_eventInstance.clearHandle();
        }
    }
}
