using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new player upgrade", menuName = "Binki/Stats/Player Upgrade")]
public class PlayerUpgrade : SerializedScriptableObject
{
    public string name;
    [TextArea]
    public string description;
    public bool hasAnimation;
    [ShowIf("hasAnimation")]
    public string animationName;
    public Sprite icon;

    public bool oneTimeOnly = false;
    [Range(0.0f, 100.0f)]
    public float chance = 100.0f;
    public List<PlayerModifier> modifiers = new List<PlayerModifier>();
}

public class PlayerModifier
{
    public ECharacterStat stat;
    public StatModifier modifer;
}