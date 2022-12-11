using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Sirenix.OdinInspector;

[NodeWidth(256)]
public abstract class BaseNode : Node 
{
    [ReadOnly, HideLabel, HorizontalGroup("Info")]
    public ENodeType type;
    [HideLabel, HorizontalGroup("Info", MaxWidth = 0.15f)]
    public int progress = 0;
    [Output(connectionType = ConnectionType.Override), HideIf("type", ENodeType.End)]
    public BaseNode next;
    [Input,
     HideIf("type", ENodeType.Begin)]
    public BaseNode prev;
    [HideInInspector]
    public bool isdone = false;

    protected bool showSounds = true;

    //[HideInInspector]
    public bool isDefault = false;

    [PropertyOrder(30), FMODUnity.EventRef, ShowIf("showSounds")]
    public string soundEventPath;
    protected SoundClip soundClip;

    protected override void Init() 
    {
        base.Init();

        OnInit();
        NodePort nextPort = GetOutputPort("next").Connection;
        if (nextPort != null)
        {
            next = nextPort.node as BaseNode;
        }

        NodePort prevPort = GetInputPort("prev").Connection;
        if (prevPort != null)
        {
            prev = prevPort.node as BaseNode;
        }

	}

    [ContextMenu("Toggle Default")]
    public void ToggleDefault()
    {
        if (!isDefault)
        {
            Init();
            BaseNode prevNode = prev;
            BaseNode nextNode = next;
            while (prevNode != null || nextNode != null)
            {
                if (prevNode != null)
                {
                    prevNode.isDefault = false;
                    prevNode = prevNode.prev;
                }

                if (nextNode != null)
                {
                    nextNode.isDefault = false;
                    nextNode = nextNode.next;
                }
            }
            isDefault = true;
        }
        else
        {
            isDefault = false;
        }
    }

    public virtual void Interrupt()
    {
        isdone = true;
    }

    public void Enter()
    {
        CreateSounds();
        OnEnter();
    }

    private void CreateSounds()
    {
        if (!string.IsNullOrEmpty(soundEventPath))
        {
            soundClip = new SoundClip(soundEventPath);
        }
    }

    public virtual void Execute()
    {
        OnExecute();
    }

    public void Exit()
    {
        if (soundClip != null)
            soundClip.Release();
        isdone = false;
        OnExit();
    }

    protected abstract void OnInit();
    protected abstract void OnEnter();
    protected abstract void OnExecute();
    protected abstract void OnExit();
}