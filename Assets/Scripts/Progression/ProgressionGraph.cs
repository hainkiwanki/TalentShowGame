using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

public class ProgressionGraph : SceneGraph
{
    public delegate void OnProgressComplete();
    public OnProgressComplete onProgressComplete;

    private BaseNode currentNode = null;
    public bool isDone = false;

    public BaseNode GetBeginNode()
    {
        for(int i = 0; i < graph.nodes.Count; i++)
        {
            if (graph.nodes[i] is BeginNode)
                return graph.nodes[i] as BaseNode;
        }
        return null;
    }

    public void Continue()
    {
        if(currentNode != null)
        {
            currentNode.Interrupt();
        }
    }

    public void ProgressSilently(int _i)
    {
        currentNode = GetBeginNode();
        if (_i == 0)
            return;

        BaseNode temp = GetBeginNode();
        while(temp != null)
        {
            temp.isdone = (temp.progress <= _i);
            temp = temp.next;
        }
        
        while(currentNode.progress != _i + 1 && currentNode != null && !currentNode.isDefault)
        {
            currentNode = currentNode.next;
        }
    }

    public int Progress(int _i)
    {
        if(_i <= 0)
        {
            currentNode = GetBeginNode();
            currentNode.Enter();
        }
        if (currentNode != null)
        {
            GameManager.Inst.controls.Player.Disable();
            GameManager.Inst.StartCoroutine(ProgressThroughTree(_i));
        }
        return _i;

    }

    IEnumerator ProgressThroughTree(int _i)
    {
        BaseNode defaultNode = null;
        currentNode.Enter();
        while(currentNode != null && currentNode.progress <= _i)
        {
            if (currentNode.isDefault)
                defaultNode = currentNode;
            while (!currentNode.isdone)
            {
                currentNode.Execute();
                yield return null;
            }

            currentNode.Exit();
            currentNode = currentNode.next;

            if (currentNode is EndNode)
                isDone = true;

            if (currentNode != null && currentNode.progress <= _i)
            {
                currentNode.Enter();
            }
        }

        if (defaultNode != null)
            currentNode = defaultNode;

        GameManager.Inst.controls.Player.Enable();
        onProgressComplete?.Invoke();
    }
}
