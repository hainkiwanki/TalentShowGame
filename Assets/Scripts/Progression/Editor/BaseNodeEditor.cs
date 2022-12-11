using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using static XNodeEditor.NodeEditor;

[CustomNodeEditor(typeof(BaseNode))]
public class BaseNodeEditor : NodeEditor
{
    private static GUIStyle editorLabelStyle;
    private static BaseNode node;

    public override void OnBodyGUI()
    {
        if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
        editorLabelStyle.normal.textColor = Color.black;
        base.OnBodyGUI();
        EditorStyles.label.normal = editorLabelStyle.normal;
    }

    public override void OnHeaderGUI()
    {
        node = target as BaseNode;
        if (node.isDefault)
        {
            var style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = Color.black;
            style.fontStyle = FontStyle.Italic;
            style.alignment = TextAnchor.MiddleCenter;
            style.padding.top = 7;
            style.padding.bottom = 7;
            GUILayout.Label(node.name + " [Default]", style);
        }
        else
        {
            base.OnHeaderGUI();
        }
    }
}
