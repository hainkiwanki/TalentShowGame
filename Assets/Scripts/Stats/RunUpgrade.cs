using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "run upgrade", menuName = "Binki/Stats/Run Upgrade")]
public class RunUpgrade : SerializedScriptableObject
{
    public string name;
    [TextArea]
    public string description;

    public List<RunModifier> modifiers = new List<RunModifier>();
    public StatModifier goldModifer;
}

public class RunModifier
{
    public ERunStat stat;
    public StatModifier modifer;
}