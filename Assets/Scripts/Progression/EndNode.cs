using System.Collections.Generic;

[NodeTint("#D80000")]
public class EndNode : BaseNode
{
    protected override void OnInit()
    {
        type = ENodeType.End;
        showSounds = false;
    }

    protected override void OnEnter() { isdone = true; }
    protected override void OnExecute() { }
    protected override void OnExit() { }
}
