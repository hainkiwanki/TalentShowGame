using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "story graph", menuName = "Binki/Progression/Story Graph")]
public class StoryGraph : NodeGraph { 
	
	public BaseNode GetBeginNode()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is BeginNode)
                return nodes[i] as BaseNode;
        }

        return null;
    }

    private BaseNode currentNode = null;
    private int progress = 0;

    public void Initialize(int _startProgress = 0)
    {
        currentNode = null;
        progress = _startProgress;

        // Quick reset
        BaseNode temp = GetBeginNode();
        while (temp.next != null)
        {
            temp.isdone = (temp.progress <= _startProgress);
            temp = temp.next;
        }

        if (_startProgress == 0)
        {
            currentNode = GetBeginNode();
        }
        else
        {
            currentNode = GetBeginNode();
            while(currentNode.progress != _startProgress + 1 && currentNode != null)
            {
                currentNode = currentNode.next;
            }
        }
    }

    public void Progress(int _i)
    {
        if (currentNode == null)
            return;

        GameManager.Inst.StartCoroutine(CO_Progress(_i));
    }   
    
    IEnumerator CO_Progress(int _i)
    {
        currentNode.Enter();
        while(currentNode != null && currentNode.progress <= _i)
        {
            while(!currentNode.isdone)
            {
                currentNode.Execute();
                yield return null;
            }

            currentNode.Exit();
            currentNode = currentNode.next;

            if(currentNode != null && currentNode.progress <= _i)
            {
                currentNode.Enter();
            }
        }
        progress = _i;
    }
}