using System.Collections.Generic;

[NodeTint("#007F0E")]
public class BeginNode : BaseNode
{
    protected override void OnInit()
    { 
        type = ENodeType.Begin;
        showSounds = false;
    }
    protected override void OnEnter() { }
    protected override void OnExecute() { isdone = true; }
    protected override void OnExit() { }
}
